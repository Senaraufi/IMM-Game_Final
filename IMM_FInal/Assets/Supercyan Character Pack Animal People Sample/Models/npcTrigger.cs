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

            if (agent == null || queueManager == null) return;

            // Store initial position and rotation
            startPosition = transform.position;
            startRotation = transform.rotation;
            
            // Show initial request message
            if (uiText != null)
            {
                ShowMessage(requestMessage);
            }
        }

        void Update()
        {
            if (!hasBeenServed && isWaitingInQueue && queueManager != null)
            {
                Vector3 targetPosition = queueManager.GetPositionInQueue(this);
                float distance = Vector3.Distance(transform.position, targetPosition);
                
                if (distance > proximityThreshold)
                {
                    // Record path points while walking to register
                    if (!hasBeenServed && pathToRegister.Count == 0 || 
                        (pathToRegister.Count > 0 && Vector3.Distance(pathToRegister[pathToRegister.Count - 1], transform.position) > 1f))
                    {
                        pathToRegister.Add(transform.position);
                    }

                    agent.SetDestination(targetPosition);
                    animController?.SetWalking(true);

                    Vector3 moveDirection = (targetPosition - transform.position).normalized;
                    if (moveDirection != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(moveDirection);
                    }
                }
                else
                {
                    agent.ResetPath();
                    animController?.SetWalking(false);
                    
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
            if (!CompareTag("Customer") || queueManager == null) return;

            isWaitingInQueue = true;
            pathToRegister.Clear(); // Clear path before starting new journey
            pathToRegister.Add(startPosition); // Add starting position
            queueManager.RegisterCustomer(this);
        }

        public void ServeFood(string foodServed)
        {
            Debug.Log($"[{gameObject.name}] ServeFood called with: {foodServed}");
            
            // Double check that we're first in queue
            if (queueManager == null || !queueManager.IsFirstInQueue(this))
            {
                Debug.Log($"[{gameObject.name}] Not first in queue, ignoring serve");
                return;
            }

            var customerRequest = GetComponent<CustomerFoodRequest>();
            if (customerRequest != null)
            {
                FoodType requestedFood = customerRequest.GetDesiredFood();
                FoodType servedFood;
                
                Debug.Log($"[{gameObject.name}] ========= SERVE FOOD COMPARISON =========");
                Debug.Log($"[{gameObject.name}] Requested Food: {requestedFood}");
                Debug.Log($"[{gameObject.name}] Served Food String: {foodServed}");
                
                // Try to parse the food type, ignoring case
                if (System.Enum.TryParse<FoodType>(foodServed, true, out servedFood))
                {
                    Debug.Log($"[{gameObject.name}] Successfully parsed served food to: {servedFood}");
                    
                    // Try multiple comparison methods
                    bool enumEqual = servedFood == requestedFood;
                    bool stringEqual = servedFood.ToString().Equals(requestedFood.ToString(), System.StringComparison.OrdinalIgnoreCase);
                    bool valueEqual = (int)servedFood == (int)requestedFood;
                    
                    Debug.Log($"[{gameObject.name}] Comparison Results:");
                    Debug.Log($"  - Enum Equality: {enumEqual}");
                    Debug.Log($"  - String Equality: {stringEqual}");
                    Debug.Log($"  - Value Equality: {valueEqual}");
                    
                    bool isCorrectFood = enumEqual || stringEqual || valueEqual;
                    Debug.Log($"[{gameObject.name}] Final Result - Is correct food? {isCorrectFood}");
                    
                    // Show response first
                    customerRequest.ShowResponse(isCorrectFood);
                    
                    if (isCorrectFood && !hasBeenServed)
                    {
                        // Mark as served and start leaving sequence
                        hasBeenServed = true;
                        isWaitingInQueue = false;
                        Debug.Log($"[{gameObject.name}] Starting leave sequence after correct food");
                        
                        // Remove from queue immediately
                        if (queueManager != null)
                        {
                            queueManager.RemoveCustomer(this);
                        }
                        
                        // Start leave sequence with a short delay
                        StartCoroutine(LeaveAfterDelay(2f));
                    }
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Failed to parse food type from string: {foodServed}");
                    customerRequest.ShowResponse(false);
                }
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] CustomerFoodRequest component missing on {gameObject.name}");
            }
        }

        private IEnumerator LeaveAfterDelay(float delay)
        {
            Debug.Log($"[{gameObject.name}] LeaveAfterDelay started with delay: {delay}");
            yield return new WaitForSeconds(delay);
            Debug.Log($"[{gameObject.name}] Delay finished, starting LeaveAndDestroy");
            
            // Remove from queue before starting to leave
            if (queueManager != null)
            {
                queueManager.RemoveCustomer(this);
            }
            
            StartCoroutine(LeaveAndDestroy());
        }

        private IEnumerator LeaveAndDestroy()
        {
            Debug.Log($"[{gameObject.name}] LeaveAndDestroy started. Path points: {pathToRegister.Count}");
            
            // First walk through recorded path points in reverse
            if (pathToRegister.Count > 1)
            {
                Debug.Log($"[{gameObject.name}] Walking through {pathToRegister.Count} path points in reverse");
                for (int i = pathToRegister.Count - 1; i >= 0; i--)
                {
                    if (agent == null) break; // Safety check
                    
                    Vector3 targetPoint = pathToRegister[i];
                    agent.SetDestination(targetPoint);
                    animController?.SetWalking(true);

                    // Wait until we reach the point or timeout
                    float pointTimeout = 3f;
                    while (agent != null && !agent.pathStatus.Equals(NavMeshPathStatus.PathInvalid) && 
                           Vector3.Distance(transform.position, targetPoint) > proximityThreshold && 
                           pointTimeout > 0)
                    {
                        pointTimeout -= Time.deltaTime;
                        yield return null;
                    }
                }
            }

            Debug.Log($"[{gameObject.name}] Returning to start position: {startPosition}");
            // Finally return to start position with original rotation
            if (agent != null)
            {
                agent.SetDestination(startPosition);
                animController?.SetWalking(true);

                // Wait until we reach the start position or timeout
                float timeout = 5f;
                while (agent != null && !agent.pathStatus.Equals(NavMeshPathStatus.PathInvalid) && 
                       Vector3.Distance(transform.position, startPosition) > proximityThreshold && 
                       timeout > 0)
                {
                    timeout -= Time.deltaTime;
                    yield return null;
                }

                // Stop walking animation
                animController?.SetWalking(false);

                // Set final rotation before destroying
                transform.rotation = startRotation;
                yield return new WaitForSeconds(0.5f);
            }
            
            Debug.Log($"[{gameObject.name}] Destroying customer object");
            Destroy(gameObject);
        }

        private void ShowMessage(string message)
        {
            if (uiText == null) return;

            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
            }

            messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
        }

        private IEnumerator ShowMessageCoroutine(string message)
        {
            uiText.text = message;
            uiText.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(messageDisplayTime);
            
            uiText.gameObject.SetActive(false);
        }
    }
}
