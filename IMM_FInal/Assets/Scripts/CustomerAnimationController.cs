using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;

        // Animation parameter names as constants
        private const string IS_WALKING = "IsWalking";
        private const string IS_IDLE = "IsIdle";
        private const string WAVE = "Wave";

        void Start()
        {
            // Get and verify animator
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] NO ANIMATOR FOUND ON OBJECT! Adding one now...");
                animator = gameObject.AddComponent<Animator>();
            }

            // Verify animator controller
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] NO ANIMATOR CONTROLLER ASSIGNED! Please assign an Animator Controller in the Unity Inspector.");
                return;
            }

            // Store initial position
            lastPosition = transform.position;
            
            // Debug log to confirm initialization
            Debug.Log($"[{gameObject.name}] CustomerAnimationController initialized successfully!");
        }

        void Update()
        {
            if (animator == null) return;

            // Check movement
            Vector3 currentPosition = transform.position;
            float movement = Vector3.Distance(currentPosition, lastPosition);
            bool wasMoving = isMoving;
            isMoving = movement > 0.001f;

            // Log state changes for debugging
            if (wasMoving != isMoving)
            {
                Debug.Log($"[{gameObject.name}] Movement state changed: {(isMoving ? "Started moving" : "Stopped moving")}");
            }

            // Set animation parameters
            animator.SetBool(IS_WALKING, isMoving);
            animator.SetBool(IS_IDLE, !isMoving);

            lastPosition = currentPosition;
        }

        public void SetWalking(bool walking)
        {
            if (animator != null)
            {
                animator.SetBool(IS_WALKING, walking);
                animator.SetBool(IS_IDLE, !walking);
            }
        }

        public void PlayHappyAnimation()
        {
            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play wave - No animator!");
                return;
            }

            animator.SetTrigger(WAVE);
        }
    }
}
