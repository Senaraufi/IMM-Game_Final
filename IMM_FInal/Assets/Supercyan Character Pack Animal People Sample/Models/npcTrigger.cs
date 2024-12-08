using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent
using TMPro; // Required for TextMeshPro

namespace SojaExiles
{
    public class animal_people_wolf_1 : MonoBehaviour
    {
        public float proximityThreshold = 1f;
        public string requestMessage = "Can I have a burger, please?";
        public TMP_Text uiText;

        private NavMeshAgent npcAgent;
        private bool moveToTrigger = false;
        private Transform playerTransform;
        private Vector3 targetPosition;
        private bool isInQueue = false;
        private RegisterQueueManager queueManager;
        private bool isRegistered = false;

        void Start()
        {
            npcAgent = GetComponent<NavMeshAgent>();
            if (npcAgent == null)
            {
                Debug.LogError("NavMeshAgent component not found on NPC!");
            }

            queueManager = RegisterQueueManager.Instance;
            if (queueManager == null)
            {
                Debug.LogError("RegisterQueueManager not found in the scene!");
            }
            else if (!isRegistered && CompareTag("Customer"))
            {
                // Register with the queue manager if we're a customer
                queueManager.RegisterCustomer(this);
                isRegistered = true;
            }

            if (uiText != null)
            {
                uiText.text = "";
            }

            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player object with tag 'Player' not found!");
            }
        }

        void Update()
        {
            if (moveToTrigger && npcAgent != null && queueManager != null)
            {
                if (!isInQueue)
                {
                    targetPosition = queueManager.GetQueuePosition(this);
                    isInQueue = true;
                }

                if (npcAgent.isActiveAndEnabled)
                {
                    npcAgent.SetDestination(targetPosition);

                    if (Vector3.Distance(transform.position, targetPosition) <= proximityThreshold)
                    {
                        if (queueManager.IsFirstInQueue(this))
                        {
                            AskForBurger();
                            FacePlayer();
                        }
                    }
                }
            }

            FacePlayer();
        }

        public void StartMovingToRegister()
        {
            if (!moveToTrigger && npcAgent != null && npcAgent.isActiveAndEnabled)
            {
                moveToTrigger = true;
                if (uiText != null)
                {
                    uiText.text = "";
                }
            }
        }

        public void UpdateQueuePosition(Vector3 newPosition)
        {
            targetPosition = newPosition;
            if (npcAgent != null && npcAgent.isActiveAndEnabled && moveToTrigger)
            {
                npcAgent.SetDestination(targetPosition);
            }
        }

        void AskForBurger()
        {
            if (uiText != null && string.IsNullOrEmpty(uiText.text))
            {
                uiText.text = requestMessage;
            }
        }

        void FacePlayer()
        {
            if (playerTransform != null)
            {
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }
            }
        }

        public void FinishOrder()
        {
            if (queueManager != null)
            {
                queueManager.RemoveFromQueue(this);
                moveToTrigger = false;
                isInQueue = false;
                if (uiText != null)
                {
                    uiText.text = "";
                }
                
                // Optional: Make NPC leave after being served
                if (npcAgent != null && npcAgent.isActiveAndEnabled)
                {
                    Vector3 exitPosition = transform.position - transform.forward * 5f;
                    npcAgent.SetDestination(exitPosition);
                }
            }
        }
    }
}
