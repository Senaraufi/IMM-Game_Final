using UnityEngine;
using UnityEditor;
using TMPro;

namespace SojaExiles
{
    public class CustomerSetup : MonoBehaviour
    {
        private Animator animator;
        private RuntimeAnimatorController defaultController;
        private RegisterQueueManager queueManager;
        private animal_people_wolf_1 wolfComponent;

        void Awake()
        {
            // Add wolf component first if it doesn't exist
            wolfComponent = GetComponent<animal_people_wolf_1>();
            if (wolfComponent == null)
            {
                wolfComponent = gameObject.AddComponent<animal_people_wolf_1>();
                Debug.Log($"[{gameObject.name}] Added wolf component");
            }
            
            // Get or add the animator component
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
                Debug.Log($"[{gameObject.name}] Added animator component");
            }
            
            // Try to find the animator controller if it's not set
            if (animator.runtimeAnimatorController == null)
            {
                // First try to load from Resources
                defaultController = Resources.Load<RuntimeAnimatorController>("CustomerAnimator");
                
                if (defaultController == null)
                {
                    // Fallback to loading from original location
                    defaultController = Resources.Load<RuntimeAnimatorController>("FreeAnimations");
                }
                
                if (defaultController != null)
                {
                    animator.runtimeAnimatorController = defaultController;
                    Debug.Log($"[{gameObject.name}] Assigned animator controller: {defaultController.name}");
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Could not find animator controller in Resources folder!");
                }
            }

            // Add CustomerAnimationController if it doesn't exist
            var animController = GetComponent<CustomerAnimationController>();
            if (animController == null)
            {
                animController = gameObject.AddComponent<CustomerAnimationController>();
                Debug.Log($"[{gameObject.name}] Added CustomerAnimationController");
            }

            // Add CustomerFoodRequest if it doesn't exist
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest == null)
            {
                foodRequest = gameObject.AddComponent<CustomerFoodRequest>();
                Debug.Log($"[{gameObject.name}] Added CustomerFoodRequest");
            }

            // Add AcceptFood if it doesn't exist
            var acceptFood = GetComponent<AcceptFood>();
            if (acceptFood == null)
            {
                acceptFood = gameObject.AddComponent<AcceptFood>();
                Debug.Log($"[{gameObject.name}] Added AcceptFood");
            }

            // Set the tag
            if (!CompareTag("Customer"))
            {
                gameObject.tag = "Customer";
                Debug.Log($"[{gameObject.name}] Set tag to Customer");
            }
        }

        void Start()
        {
            // Get queue manager
            queueManager = RegisterQueueManager.Instance;
            if (queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] RegisterQueueManager not found!");
                return;
            }

            // Register with queue manager if we're a customer
            if (CompareTag("Customer"))
            {
                queueManager.RegisterCustomer(wolfComponent);
                Debug.Log($"[{gameObject.name}] Registered with queue manager");
            }
        }
    }
}
