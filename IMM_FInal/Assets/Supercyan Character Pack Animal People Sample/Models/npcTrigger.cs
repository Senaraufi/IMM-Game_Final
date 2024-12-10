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
        private CustomerAnimationController animController;
        private bool isWaitingInQueue = false;
        private bool hasBeenServed = false;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private List<Vector3> pathToRegister = new List<Vector3>();
        private Coroutine messageCoroutine;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animController = GetComponent<CustomerAnimationController>();
            queueManager = RegisterQueueManager.Instance;

            if (agent == null || queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing required components!");
                return;
            }

            // Store initial position and rotation
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            Debug.Log($"[{gameObject.name}] Initialized at position: {startPosition}");
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
                    float currentSpeed = agent.velocity.magnitude;
                    
                    // Update animation based on speed and distance
                    if (animController != null)
                    {
                        // Run if we're far from the target, walk if we're close
                        float runDistance = 5f; // Distance at which to start running
                        if (distance > runDistance)
                        {
                            agent.speed = 5f; // Increase speed for running
                            animController.SetMovementState(3f); // Running
                        }
                        else
                        {
                            agent.speed = 2f; // Normal walking speed
                            animController.SetMovementState(1f); // Walking
                        }
                    }

                    // Update rotation
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
                        animController.SetMovementState(0f); // Idle
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

        public void StartMovingToRegister()
        {
            if (!CompareTag("Customer") || queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot move to register - invalid setup");
                return;
            }

            isWaitingInQueue = true;
            pathToRegister.Clear(); // Clear path before starting new journey
            pathToRegister.Add(startPosition); // Add starting position
            queueManager.RegisterCustomer(this);
            Debug.Log($"[{gameObject.name}] Started moving to register");
        }

        public void ServeFood(string foodServed)
        {
            Debug.Log($"[{gameObject.name}] ServeFood called with: {foodServed}");
            
            if (queueManager != null)
            {
                queueManager.CustomerLeaving(this);
            }

            hasBeenServed = true;
            isWaitingInQueue = false;

            // Start returning to original position
            StartCoroutine(ReturnToStart());
        }

        private IEnumerator ReturnToStart()
        {
            Debug.Log($"[{gameObject.name}] Starting return journey");

            // Reverse the path we took to get here
            pathToRegister.Reverse();
            
            foreach (Vector3 point in pathToRegister)
            {
                agent.SetDestination(point);
                float distance = Vector3.Distance(transform.position, point);
                
                // Run if we're far from the point
                if (distance > 5f)
                {
                    agent.speed = 5f;
                    animController?.SetMovementState(3f); // Running
                }
                else
                {
                    agent.speed = 2f;
                    animController?.SetMovementState(1f); // Walking
                }

                while (Vector3.Distance(transform.position, point) > proximityThreshold)
                {
                    yield return null;
                }
            }

            // Finally return to exact start position and rotation
            agent.SetDestination(startPosition);
            float finalDistance = Vector3.Distance(transform.position, startPosition);
            
            // Run back to start position if far enough
            if (finalDistance > 5f)
            {
                agent.speed = 5f;
                animController?.SetMovementState(3f);
            }
            else
            {
                agent.speed = 2f;
                animController?.SetMovementState(1f);
            }

            while (Vector3.Distance(transform.position, startPosition) > proximityThreshold)
            {
                yield return null;
            }

            transform.rotation = startRotation;
            animController?.SetMovementState(0f); // Set to idle at the end
            
            // Reset state
            pathToRegister.Clear();
            hasBeenServed = false;
            
            Debug.Log($"[{gameObject.name}] Returned to start position");
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
                uiText.gameObject.SetActive(true);
                yield return new WaitForSeconds(messageDisplayTime);
                uiText.gameObject.SetActive(false);
            }
        }

        public bool GetIsFirstInQueue()
        {
            return queueManager != null && queueManager.IsFirstInQueue(this);
        }
    }
}
