using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;
        private bool isRunning;

        // Animation parameter names as constants
        private const string IS_WALKING = "IsWalking";
        private const string IS_RUNNING = "IsRunning";
        private const string IS_IDLE = "IsIdle";
        private const string WAVE = "Wave";
        private const string HAPPY = "Happy";
        private const string ANGRY = "Angry";

        void Awake()
        {
            SetupAnimator();
        }

        void Start()
        {
            // Verify animator is properly set up
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] Animator or controller is still null in Start! Attempting to fix...");
                SetupAnimator();
            }

            // Store initial position
            lastPosition = transform.position;
            
            // Debug log to confirm initialization
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                Debug.Log($"[{gameObject.name}] CustomerAnimationController initialized successfully!");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Failed to initialize animator properly!");
            }
        }

        private void SetupAnimator()
        {
            // Get and verify animator
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.Log($"[{gameObject.name}] No Animator found - adding one");
                animator = gameObject.AddComponent<Animator>();
            }

            // Try to find the animator controller if it's not set
            if (animator.runtimeAnimatorController == null)
            {
                Debug.Log($"[{gameObject.name}] Looking for animator controller...");
                
                // First try Resources folder
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("FreeAnimations");
                
                if (controller == null)
                {
                    Debug.Log($"[{gameObject.name}] Controller not found in Resources, checking original location...");
                    // Try original location
                    controller = Resources.Load<RuntimeAnimatorController>("Supercyan Character Pack Animal People Sample/AnimatorControllers/FreeAnimations");
                }

                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                    Debug.Log($"[{gameObject.name}] Successfully loaded animator controller!");
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Failed to find animator controller in any location!");
                }
            }
            
            // Set default animation state
            if (animator.runtimeAnimatorController != null)
            {
                animator.SetBool(IS_IDLE, true);
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(IS_RUNNING, false);
            }
        }

        void Update()
        {
            if (animator == null || animator.runtimeAnimatorController == null) return;

            // Check if position has changed
            Vector3 currentPosition = transform.position;
            float moveSpeed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            isMoving = moveSpeed > 0.01f;
            
            // Update animations based on movement speed
            if (isMoving)
            {
                // If moving faster than 2 units per second, use running animation
                isRunning = moveSpeed > 2f;
                animator.SetBool(IS_RUNNING, isRunning);
                animator.SetBool(IS_WALKING, !isRunning);
                animator.SetBool(IS_IDLE, false);
            }
            else
            {
                // If not moving, go to idle
                animator.SetBool(IS_IDLE, true);
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(IS_RUNNING, false);
            }
            
            // Update last position
            lastPosition = currentPosition;
        }

        public void SetMovementState(float speed)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot set movement state - No animator!");
                return;
            }

            if (speed <= 0)
            {
                // Idle
                animator.SetBool(IS_IDLE, true);
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(IS_RUNNING, false);
            }
            else if (speed <= 2f)
            {
                // Walking
                animator.SetBool(IS_IDLE, false);
                animator.SetBool(IS_WALKING, true);
                animator.SetBool(IS_RUNNING, false);
            }
            else
            {
                // Running
                animator.SetBool(IS_IDLE, false);
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(IS_RUNNING, true);
            }
        }

        public void PlayHappyAnimation()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play happy animation - No animator!");
                return;
            }

            animator.SetTrigger(HAPPY);
            Debug.Log($"[{gameObject.name}] Playing happy animation");
        }

        public void PlayWaveAnimation()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play wave animation - No animator!");
                return;
            }

            animator.SetTrigger(WAVE);
            Debug.Log($"[{gameObject.name}] Playing wave animation");
        }

        public void PlayAngryAnimation()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play angry animation - No animator!");
                return;
            }

            animator.SetTrigger(ANGRY);
            Debug.Log($"[{gameObject.name}] Playing angry animation");
        }
    }
}
