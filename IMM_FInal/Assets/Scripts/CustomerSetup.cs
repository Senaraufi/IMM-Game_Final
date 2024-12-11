using UnityEngine;
using TMPro;

namespace SojaExiles
{
    public class CustomerSetup : MonoBehaviour
    {
        private Animator animator;
        private RuntimeAnimatorController defaultController;
        private RegisterQueueManager queueManager;
        private animal_people_wolf_1 wolfComponent;
        private CustomerBehavior customerBehavior;

        void Awake()
        {
            // Add required components in specific order
            SetupComponents();
            SetupAnimator();
            SetupTag();
        }

        private void SetupComponents()
        {
            // 1. Add wolf component first
            wolfComponent = GetComponent<animal_people_wolf_1>();
            if (wolfComponent == null)
            {
                wolfComponent = gameObject.AddComponent<animal_people_wolf_1>();
                Debug.Log($"[{gameObject.name}] Added wolf component");
            }

            // 2. Add CustomerBehavior
            customerBehavior = GetComponent<CustomerBehavior>();
            if (customerBehavior == null)
            {
                customerBehavior = gameObject.AddComponent<CustomerBehavior>();
                Debug.Log($"[{gameObject.name}] Added CustomerBehavior");
            }

            // 3. Add CustomerFoodRequest
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest == null)
            {
                foodRequest = gameObject.AddComponent<CustomerFoodRequest>();
                Debug.Log($"[{gameObject.name}] Added CustomerFoodRequest");
            }

            // 4. Add AcceptFood
            var acceptFood = GetComponent<AcceptFood>();
            if (acceptFood == null)
            {
                acceptFood = gameObject.AddComponent<AcceptFood>();
                Debug.Log($"[{gameObject.name}] Added AcceptFood");
            }
        }

        private void SetupAnimator()
        {
            // Get or add animator
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
                Debug.Log($"[{gameObject.name}] Added Animator");
            }

            // Set up animator controller
            if (animator.runtimeAnimatorController == null)
            {
                // Try to load controllers from Resources folder
                defaultController = Resources.Load<RuntimeAnimatorController>("CustomerAnimator");
                
                if (defaultController == null)
                {
                    defaultController = Resources.Load<RuntimeAnimatorController>("WolfController");
                }
                
                if (defaultController == null)
                {
                    defaultController = Resources.Load<RuntimeAnimatorController>("FreeAnimations");
                }

                if (defaultController != null)
                {
                    animator.runtimeAnimatorController = defaultController;
                    Debug.Log($"[{gameObject.name}] Assigned animator controller: {defaultController.name}");
                    animator.SetBool("Walk", false);
                    animator.SetBool("Run", false);
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Could not find any suitable animator controller!");
                }
            }
        }

        private void SetupTag()
        {
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
