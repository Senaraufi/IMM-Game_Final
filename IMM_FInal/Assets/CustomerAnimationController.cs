using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;
        
        // Animation parameter names
        private static readonly string IS_WALKING = "IsWalking";
        private static readonly string IS_IDLE = "IsIdle";
        private static readonly string WAVE = "Wave";
        
        void Start()
        {
            animator = GetComponent<Animator>();
            lastPosition = transform.position;
            
            // Ensure we have an animator component
            if (animator == null)
            {
                Debug.LogError("Animator component missing from customer!");
                return;
            }
        }
        
        void Update()
        {
            // Check if the customer is moving by comparing current position to last position
            Vector3 currentPosition = transform.position;
            float movement = Vector3.Distance(currentPosition, lastPosition);
            isMoving = movement > 0.01f; // Threshold for movement detection
            
            // Update animation states
            UpdateAnimationState();
            
            lastPosition = currentPosition;
        }
        
        private void UpdateAnimationState()
        {
            if (animator == null) return;
            
            // Set walking animation when moving
            animator.SetBool(IS_WALKING, isMoving);
            animator.SetBool(IS_IDLE, !isMoving);
        }
        
        // Call this when customer receives correct food
        public void PlayHappyAnimation()
        {
            if (animator == null) return;
            animator.SetTrigger(WAVE);
        }
    }
}
