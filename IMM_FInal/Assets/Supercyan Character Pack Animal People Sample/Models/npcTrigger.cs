using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System.Collections;

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
        private Coroutine messageCoroutine;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animController = GetComponent<CustomerAnimationController>();
            queueManager = RegisterQueueManager.Instance;

            if (agent == null || queueManager == null) return;

            startPosition = transform.position;
            
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
            queueManager.RegisterCustomer(this);
        }

        public void ServeFood(string foodServed)
        {
            if (!queueManager.IsFirstInQueue(this)) return;

            // Check if correct food was served
            bool isCorrectFood = foodServed.ToLower().Contains("burger");
            
            // Show appropriate message
            if (isCorrectFood)
            {
                ShowMessage("Thank you!");
                StartCoroutine(LeaveAfterDelay(1f)); // Leave after saying thank you
            }
            else
            {
                ShowMessage("NO!");
                // Don't leave, wait for correct food
                return;
            }

            hasBeenServed = isCorrectFood;
            if (isCorrectFood)
            {
                isWaitingInQueue = false;
                queueManager.CustomerLeaving(this);
            }
        }

        private IEnumerator LeaveAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(LeaveAndDestroy());
        }

        private void ShowMessage(string message)
        {
            if (uiText == null) return;

            // Stop any existing message coroutine
            if (messageCoroutine != null)
            {
                StopCoroutine(messageCoroutine);
            }

            // Start new message coroutine
            messageCoroutine = StartCoroutine(ShowMessageCoroutine(message));
        }

        private IEnumerator ShowMessageCoroutine(string message)
        {
            uiText.text = message;
            uiText.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(messageDisplayTime);
            
            uiText.gameObject.SetActive(false);
        }

        private IEnumerator LeaveAndDestroy()
        {
            agent.SetDestination(startPosition);
            animController?.SetWalking(true);

            float timeout = 5f;
            while (Vector3.Distance(transform.position, startPosition) > proximityThreshold && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
