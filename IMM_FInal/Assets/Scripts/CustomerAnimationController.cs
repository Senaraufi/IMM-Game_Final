using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;
        private UnityEngine.AI.NavMeshAgent agent;

        // Animation parameter names
        private const string PARAM_IS_WALKING = "IsWalking";
        private const string PARAM_WAVE = "Wave";

        void Start()
        {
            // Get components
            animator = GetComponent<Animator>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Animator component missing!");
                return;
            }

            // Store initial position
            lastPosition = transform.position;
            
            // Ensure we start in idle state
            animator.SetBool(PARAM_IS_WALKING, false);
            Debug.Log($"[{gameObject.name}] Animation controller initialized");
        }

        void Update()
        {
            if (animator == null) return;

            // Check if moving using NavMeshAgent velocity
            bool wasMoving = isMoving;
            if (agent != null)
            {
                isMoving = agent.velocity.magnitude > 0.1f;
            }
            else
            {
                // Fallback to position check if no NavMeshAgent
                Vector3 currentPosition = transform.position;
                float movement = Vector3.Distance(currentPosition, lastPosition);
                isMoving = movement > 0.001f;
                lastPosition = currentPosition;
            }

            // Update animation state if movement changed
            if (wasMoving != isMoving)
            {
                animator.SetBool(PARAM_IS_WALKING, isMoving);
                Debug.Log($"[{gameObject.name}] Set to {(isMoving ? "walking" : "idle")} animation");
            }
        }

        void OnAnimatorMove()
        {
            // This is called when root motion is handled by script
            if (animator == null || agent == null) return;

            // If we're using NavMeshAgent for movement, we don't want root motion to affect position
            if (agent.enabled && agent.isOnNavMesh)
            {
                // Get the root motion delta
                Vector3 rootMotion = animator.deltaPosition;
                
                // Apply the root motion to the NavMeshAgent's velocity instead of directly moving
                if (rootMotion.magnitude > 0)
                {
                    agent.velocity = rootMotion / Time.deltaTime;
                }
            }
        }

        public void SetWalking(bool walking)
        {
            if (animator == null) return;

            animator.SetBool(PARAM_IS_WALKING, walking);
            Debug.Log($"[{gameObject.name}] Set walking: {walking}");
        }

        public void PlayHappyAnimation()
        {
            if (animator == null) return;

            animator.SetTrigger(PARAM_WAVE);
            Debug.Log($"[{gameObject.name}] Playing wave animation");
        }

        public void PlayAngryAnimation()
        {
            PlayHappyAnimation(); // Using wave for now
        }
    }
}
