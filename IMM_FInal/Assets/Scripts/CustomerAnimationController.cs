using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;

        // Animation parameter names as constants
        private const string IS_WALKING = "Walk";
        private const string IS_IDLE = "Idle";
        private const string WAVE = "Wave";
        private const string HAPPY = "Happy";
        private const string ANGRY = "Angry";

        void Start()
        {
            // Get animator component
            animator = GetComponent<Animator>();

            // Verify animator is properly set up
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                SetupAnimator();
            }

            // Store initial position
            lastPosition = transform.position;
        }

        private void SetupAnimator()
        {
            // Try to get animator if it's missing
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
            }

            // Try to find the animator controller if it's not set
            if (animator.runtimeAnimatorController == null)
            {
                // First try Resources folder
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("FreeAnimations");

                if (controller == null)
                {
                    // Try original location
                    controller = Resources.Load<RuntimeAnimatorController>("Supercyan Character Pack Animal People Sample/AnimatorControllers/FreeAnimations");
                }

                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }

            // Set initial animation state
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetBool(IS_IDLE, true);
                animator.SetBool(IS_WALKING, false);
            }
        }

        void Update()
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                return;
            }

            // Get current position
            Vector3 currentPosition = transform.position;

            // Calculate movement
            float moveSpeed = Vector3.Distance(currentPosition, lastPosition) / Time.deltaTime;
            isMoving = moveSpeed > 0.01f;

            // Update animations based on movement
            if (isMoving)
            {
                animator.SetBool(IS_WALKING, true);
                animator.SetBool(IS_IDLE, false);
            }
            else
            {
                animator.SetBool(IS_IDLE, true);
                animator.SetBool(IS_WALKING, false);
            }

            // Update last position
            lastPosition = currentPosition;
        }

        public void PlayWaveAnimation()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetTrigger(WAVE);
            }
        }

        public void PlayHappyAnimation()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetTrigger(HAPPY);
            }
        }

        public void PlayAngryAnimation()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetTrigger(ANGRY);
            }
        }

        public void StopMoving()
        {
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.SetBool(IS_WALKING, false);
                animator.SetBool(IS_IDLE, true);
            }
        }
    }
}
