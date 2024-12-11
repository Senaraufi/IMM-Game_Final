using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

namespace SojaExiles
{
    public class CustomerBehavior : MonoBehaviour
    {
        public float proximityThreshold = 0.5f;
        public float messageDisplayTime = 2f;
        public float greetingWaveTime = 2f;  // How long to wave when reaching counter
        public float responseWaitTime = 2f;   // How long to wait after waving/showing response

        private NavMeshAgent agent;
        private Animator animator;
        private RegisterQueueManager queueManager;
        private bool isWaitingInQueue = false;
        private bool hasBeenServed = false;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private bool hasGreeted = false;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            queueManager = RegisterQueueManager.Instance;

            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing Animator component!");
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
                    agent.SetDestination(targetPosition);
                    
                    // Update animation based on movement
                    if (animator != null)
                    {
                        bool isMoving = agent.velocity.magnitude > 0.1f;
                        SetAnimationState(isMoving);
                        
                        // Adjust speed based on distance
                        float speedMultiplier = distance > 5f ? 1.5f : 1f;
                        agent.speed = 3.5f * speedMultiplier;
                        hasGreeted = false; // Reset greeting flag while moving
                    }
                }
                else
                {
                    if (agent.hasPath)
                    {
                        agent.ResetPath();
                        if (animator != null)
                        {
                            SetAnimationState(false);
                            
                            // Wave when first reaching the counter
                            if (!hasGreeted)
                            {
                                StartCoroutine(GreetingSequence());
                                hasGreeted = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (agent != null)
                {
                    // Set walking animation based on whether the agent is moving
                    SetAnimationState(agent.velocity.magnitude > 0.1f);
                }
            }
        }

        private void SetAnimationState(bool isWalking)
        {
            if (animator != null)
            {
                animator.SetBool("Walk", isWalking);
            }
        }

        private IEnumerator GreetingSequence()
        {
            // Wave when reaching counter
            PlayWaveAnimation();
            
            // Wait for wave animation
            yield return new WaitForSeconds(greetingWaveTime);
            
            // Generate and show food request
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest != null)
            {
                foodRequest.GenerateRandomFoodRequest();
                foodRequest.ShowRequest();
            }
        }

        public void PlayWaveAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Wave");
            }
        }

        public void PlayHappyAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Happy");
            }
        }

        public void PlayAngryAnimation()
        {
            if (animator != null)
            {
                animator.SetTrigger("Angry");
            }
        }

        public void PlayHappyAnimationOld()
        {
            if (animator != null)
            {
                animator.SetTrigger("Wave");
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                StartCoroutine(WaitAfterResponse());
            }
        }

        public void PlayAngryAnimationOld()
        {
            if (animator != null)
            {
                animator.SetTrigger("No");
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                StartCoroutine(WaitAfterResponse());
            }
        }

        private IEnumerator WaitAfterResponse()
        {
            // Wait for response animation to complete
            yield return new WaitForSeconds(responseWaitTime);
            
            // Start returning
            StartReturnToStart();
        }

        public void StartReturnToStart()
        {
            if (!hasBeenServed)
            {
                hasBeenServed = true;
                if (queueManager != null)
                {
                    queueManager.CustomerLeaving(this);
                }

                // Start moving back
                if (agent != null)
                {
                    agent.SetDestination(startPosition);
                    StartCoroutine(WaitForReturnToStart());
                }
            }
        }

        private IEnumerator WaitForReturnToStart()
        {
            // Set walking animation while returning
            if (animator != null)
            {
                SetAnimationState(true);
            }

            while (Vector3.Distance(transform.position, startPosition) > proximityThreshold)
            {
                yield return null;
            }

            // Reached start position
            if (animator != null)
            {
                SetAnimationState(false);
            }
            
            // Reset rotation
            transform.rotation = startRotation;
            
            // Reset state
            hasBeenServed = false;
            isWaitingInQueue = false;
            hasGreeted = false;
        }

        public void EnterQueue()
        {
            if (hasBeenServed || isWaitingInQueue) return;
            
            isWaitingInQueue = true;
            queueManager.RegisterCustomer(this);
        }

        public bool HasBeenServed()
        {
            return hasBeenServed;
        }

        public void ResetServed()
        {
            hasBeenServed = false;
        }

        public void StartMovingToRegister()  // Adding this back for compatibility
        {
            EnterQueue();
        }

        public void ServeFood(string foodServed)  // Adding this back for compatibility
        {
            if (hasBeenServed) return;

            hasBeenServed = true;
            isWaitingInQueue = false;

            if (queueManager != null)
            {
                queueManager.CustomerLeaving(this);
            }
        }
    }
}
