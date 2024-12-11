using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace SojaExiles
{
    public class CustomerBehavior : MonoBehaviour
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

        void Update()
        {
            if (!hasBeenServed && isWaitingInQueue && queueManager != null)
            {
                Vector3 targetPosition = queueManager.GetQueuePosition(this);
                float distance = Vector3.Distance(transform.position, targetPosition);
                
                if (distance > proximityThreshold)
                {
                    // Only record path points if we're far enough from the last recorded point
                    if (pathToRegister.Count == 0 || 
                        Vector3.Distance(pathToRegister[pathToRegister.Count - 1], transform.position) > 2f)
                    {
                        pathToRegister.Add(transform.position);
                    }

                    // Set destination and calculate speed
                    agent.SetDestination(targetPosition);
                    
                    // Update animation and speed based on distance
                    if (animController != null)
                    {
                        bool shouldWalk = distance > proximityThreshold;
                        if (shouldWalk != animController.GetBool("Walk"))
                        {
                            animController.SetBool("Walk", shouldWalk);
                            animController.SetBool("Idle", !shouldWalk);
                            agent.speed = distance > 5f ? 5f : 2f;
                        }
                    }

                    // Only update rotation if we need to move
                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
                    if (moveDirection != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, 
                            Quaternion.LookRotation(moveDirection), Time.deltaTime * 5f);
                    }
                }
                else
                {
                    if (agent.hasPath)
                    {
                        agent.ResetPath();
                        if (animController != null)
                        {
                            animController.SetBool("Walk", false);
                            animController.SetBool("Idle", true);
                        }
                    }
                    
                    // Only update camera facing when needed
                    if (Camera.main != null)
                    {
                        Vector3 directionToCamera = Camera.main.transform.position - transform.position;
                        directionToCamera.y = 0;
                        if (directionToCamera != Vector3.zero)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation,
                                Quaternion.LookRotation(directionToCamera), Time.deltaTime * 5f);
                        }
                    }
                }
            }
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

        public void ServeFood(string foodServed)
        {
            if (hasBeenServed) return;

            hasBeenServed = true;
            servedFood = foodServed;
            isWaitingInQueue = false;

            // Leave the queue
            if (queueManager != null)
            {
                queueManager.CustomerLeaving(this);
            }
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

        public void StartReturnToStart()
        {
            StartCoroutine(ReturnToStart());
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
            yield return new WaitForSeconds(3.0f); // Wait for response animation to complete
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

        public void PlayHappyAnimation()
        {
            if (animController != null)
            {
                animController.SetBool("Happy", true);
                StartCoroutine(ResetAnimation("Happy"));
            }
        }

        public void PlayAngryAnimation()
        {
            if (animController != null)
            {
                animController.SetBool("Angry", true);
                StartCoroutine(ResetAnimation("Angry"));
            }
        }

        private IEnumerator ResetAnimation(string animationName)
        {
            yield return new WaitForSeconds(2f);
            if (animController != null)
            {
                animController.SetBool(animationName, false);
            }
        }
    }
}
