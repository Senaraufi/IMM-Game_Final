using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace SojaExiles
{
    public class animal_people_wolf_1 : MonoBehaviour
    {
        public float proximityThreshold = 0.5f;
        public string requestMessage = "Can I have a burger, please?";
        public TMP_Text uiText;
        public float messageDisplayTime = 2f; // How long to show messages

        private NavMeshAgent agent;
        private RegisterQueueManager queueManager;
        private Animator animController;
        private bool isWaitingInQueue = false;
        private bool hasBeenServed = false;
        private string servedFood = ""; // Added to track what food was served
        private Vector3 startPosition;
        private Quaternion startRotation;
        private List<Vector3> pathToRegister = new List<Vector3>();
        private Coroutine messageCoroutine;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animController = GetComponent<Animator>();
            queueManager = RegisterQueueManager.Instance;

            // Initialize the food request when the customer spawns
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest != null)
            {
                foodRequest.GenerateRandomFoodRequest();
            }

            if (agent == null || queueManager == null)
            {
                return;
            }

            // Store initial position and rotation
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        public void StartMovingToRegister()
        {
            if (!CompareTag("Customer") || queueManager == null)
            {
                return;
            }

            isWaitingInQueue = true;
            pathToRegister.Clear();
            pathToRegister.Add(startPosition);
            queueManager.RegisterCustomer(this);

            // Show the food request when starting to move
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest != null)
            {
                foodRequest.GenerateRandomFoodRequest(); // Generate a new request
                foodRequest.ShowRequest(); // Show it immediately
            }
        }

        void Update()
        {
            if (!hasBeenServed && isWaitingInQueue && queueManager != null)
            {
                Vector3 targetPosition = queueManager.GetQueuePosition(this);
                float distance = Vector3.Distance(transform.position, targetPosition);
                
                if (distance > proximityThreshold)
                {
                    // Record path points while walking to register
                    if (!hasBeenServed && pathToRegister.Count == 0 || 
                        (pathToRegister.Count > 0 && Vector3.Distance(pathToRegister[pathToRegister.Count - 1], transform.position) > 1f))
                    {
                        pathToRegister.Add(transform.position);
                    }

                    // Set destination and calculate speed
                    agent.SetDestination(targetPosition);
                    
                    // Update animation and speed based on distance
                    if (animController != null)
                    {
                        if (distance > 5f)
                        {
                            agent.speed = 5f;
                            animController.SetBool("Walk", true);
                        }
                        else
                        {
                            agent.speed = 2f;
                            animController.SetBool("Walk", true);
                        }
                    }

                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
                    if (moveDirection != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(moveDirection);
                    }
                }
                else
                {
                    agent.ResetPath();
                    if (animController != null)
                    {
                        animController.SetBool("Walk", false);
                        animController.SetBool("Idle", true);
                    }
                    
                    if (Camera.main != null)
                    {
                        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
                        directionToCamera.y = 0;
                        if (directionToCamera != Vector3.zero)
                        {
                            transform.rotation = Quaternion.LookRotation(directionToCamera);
                        }
                    }
                }
            }
        }

        public void ServeFood(string foodServed)
        {
            if (queueManager != null)
            {
                queueManager.CustomerLeaving(this);
            }

            // Get the food request component
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest != null)
            {
                // Show a response based on whether they got the correct food
                bool isCorrectFood = foodServed.ToLower() == foodRequest.GetDesiredFood().ToString().ToLower();
                foodRequest.ShowResponse(isCorrectFood);
                
                // Wait a moment before hiding the request
                StartCoroutine(HideRequestAfterDelay(foodRequest, 2f));
            }

            hasBeenServed = true;
            servedFood = foodServed;
            isWaitingInQueue = false;

            StartCoroutine(ReturnToStart());
        }

        private IEnumerator HideRequestAfterDelay(CustomerFoodRequest request, float delay)
        {
            yield return new WaitForSeconds(delay);
            request.HideRequest();
        }

        public bool HasBeenServed()
        {
            return hasBeenServed;
        }

        public string GetServedFood()
        {
            return servedFood;
        }

        public void ResetServed()
        {
            hasBeenServed = false;
            servedFood = "";
        }

        private IEnumerator ReturnToStart()
        {
            // Reverse the path we took to get here
            pathToRegister.Reverse();
            
            foreach (Vector3 point in pathToRegister)
            {
                agent.SetDestination(point);
                float distance = Vector3.Distance(transform.position, point);
                
                // Set speed and animation based on distance
                if (distance > 5f)
                {
                    agent.speed = 5f;
                    if (animController != null)
                    {
                        animController.SetBool("Walk", true);
                        animController.SetBool("Idle", false);
                    }
                }
                else
                {
                    agent.speed = 2f;
                    if (animController != null)
                    {
                        animController.SetBool("Walk", true);
                        animController.SetBool("Idle", false);
                    }
                }

                while (Vector3.Distance(transform.position, point) > proximityThreshold)
                {
                    yield return null;
                }
            }

            // Finally return to exact start position and rotation
            agent.SetDestination(startPosition);
            float finalDistance = Vector3.Distance(transform.position, startPosition);
            
            // Set speed and animation for final return
            if (finalDistance > 5f)
            {
                agent.speed = 5f;
            }
            else
            {
                agent.speed = 2f;
            }

            if (animController != null)
            {
                animController.SetBool("Walk", true);
                animController.SetBool("Idle", false);
            }

            while (Vector3.Distance(transform.position, startPosition) > proximityThreshold)
            {
                yield return null;
            }

            transform.rotation = startRotation;
            if (animController != null)
            {
                animController.SetBool("Walk", false);
                animController.SetBool("Idle", true);
            }
            
            // Reset state
            pathToRegister.Clear();
            Destroy(gameObject, 0.5f);
        }

        private void ShowMessage(string message)
        {
            if (uiText != null)
            {
                if (messageCoroutine != null)
                {
                    StopCoroutine(messageCoroutine);
                }
                messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
            }
        }

        private IEnumerator ShowMessageCoroutine(string message)
        {
            if (uiText != null)
            {
                uiText.text = message;
                uiText.enabled = true;
                yield return new WaitForSeconds(messageDisplayTime);
                uiText.text = "";
                uiText.enabled = false;
            }
        }
    }
}
