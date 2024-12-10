using UnityEngine;

namespace SojaExiles
{
    public class CustomerAnimationController : MonoBehaviour
    {
        private Animator animator;
        private Vector3 lastPosition;
        private bool isMoving;
<<<<<<< HEAD
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
=======

        // Animation parameter names as constants
        private const string IS_WALKING = "IsWalking";
        private const string IS_IDLE = "IsIdle";
        private const string WAVE = "Wave";
        private const string ANGRY = "Angry";

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
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
                return;
            }

            // Store initial position
            lastPosition = transform.position;
            
<<<<<<< HEAD
            // Ensure we start in idle state
            animator.SetBool(PARAM_IS_WALKING, false);
            Debug.Log($"[{gameObject.name}] Animation controller initialized");
=======
            // Debug log to confirm initialization
            Debug.Log($"[{gameObject.name}] CustomerAnimationController initialized successfully!");
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
        }

        void Update()
        {
            if (animator == null) return;

<<<<<<< HEAD
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
=======
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
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
        }

        public void SetWalking(bool walking)
        {
<<<<<<< HEAD
            if (animator == null) return;

            animator.SetBool(PARAM_IS_WALKING, walking);
            Debug.Log($"[{gameObject.name}] Set walking: {walking}");
=======
            if (animator != null)
            {
                animator.SetBool(IS_WALKING, walking);
                animator.SetBool(IS_IDLE, !walking);
            }
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
        }

        public void PlayHappyAnimation()
        {
<<<<<<< HEAD
            if (animator == null) return;

            animator.SetTrigger(PARAM_WAVE);
            Debug.Log($"[{gameObject.name}] Playing wave animation");
=======
            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play wave - No animator!");
                return;
            }

            animator.SetTrigger(WAVE);
            Debug.Log($"[{gameObject.name}] Playing happy (wave) animation");
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
        }

        public void PlayAngryAnimation()
        {
<<<<<<< HEAD
            PlayHappyAnimation(); // Using wave for now
=======
            if (animator == null)
            {
                Debug.LogError($"[{gameObject.name}] Cannot play angry animation - No animator!");
                return;
            }

            // For now, we'll just use the wave animation since that's what we have
            // You can replace this with a proper angry animation trigger when you have one
            animator.SetTrigger(WAVE);
            Debug.Log($"[{gameObject.name}] Playing angry animation (using wave for now)");
>>>>>>> b77c0cbddc1c021d93d61622353fb131c2a0f838
        }
    }
}
