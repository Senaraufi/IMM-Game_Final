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
                Debug.LogError($"[{gameObject.name}] NO ANIMATOR FOUND ON OBJECT!");
                return;
            }

            // Verify animator controller
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogError($"[{gameObject.name}] NO ANIMATOR CONTROLLER ASSIGNED!");
                return;
            }

            // Log initial setup
            Debug.Log($"[{gameObject.name}] Animator initialized successfully");
            Debug.Log($"[{gameObject.name}] Controller: {animator.runtimeAnimatorController.name}");
            Debug.Log($"[{gameObject.name}] Apply Root Motion: {animator.applyRootMotion}");

            // Log all parameters
            foreach (var param in animator.parameters)
            {
                Debug.Log($"[{gameObject.name}] Parameter found: {param.name} ({param.type})");
            }

            // Store initial position
            lastPosition = transform.position;
        }

        void Update()
        {
            if (animator == null) return;

            // Check movement
            Vector3 currentPosition = transform.position;
            float movement = Vector3.Distance(currentPosition, lastPosition);
            bool wasMoving = isMoving;
            isMoving = movement > 0.001f;

            // Log movement changes
            if (wasMoving != isMoving)
            {
                Debug.Log($"[{gameObject.name}] Movement changed: {(isMoving ? "Started moving" : "Stopped moving")}");
                Debug.Log($"[{gameObject.name}] Movement distance: {movement}");
            }

            // Set animation parameters
            animator.SetBool(IS_WALKING, isMoving);
            animator.SetBool(IS_IDLE, !isMoving);

            // Log current animation state
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (isMoving)
            {
                Debug.Log($"[{gameObject.name}] Current State: {stateInfo.shortNameHash}, IsWalking: {animator.GetBool(IS_WALKING)}");
            }

            lastPosition = currentPosition;
        }

        public void PlayHappyAnimation()
        {
            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play wave - No animator!");
                return;
            }

            Debug.Log($"[{gameObject.name}] TRIGGERING WAVE ANIMATION");
            animator.SetTrigger(WAVE);

            // Log animation state after triggering wave
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log($"[{gameObject.name}] After wave trigger - State hash: {stateInfo.shortNameHash}");
            Debug.Log($"[{gameObject.name}] Is wave parameter triggered: {animator.GetBool(WAVE)}");
        }
    }
}
