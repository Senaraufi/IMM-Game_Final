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
<<<<<<< Updated upstream
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
=======
            // Add required components in specific order
            SetupComponents();
            SetupAnimator();
            SetupTag();
        }

        private void SetupComponents()
        {
            // 1. Add CustomerBehavior first
            customerBehavior = GetComponent<CustomerBehavior>();
            if (customerBehavior == null)
            {
                customerBehavior = gameObject.AddComponent<CustomerBehavior>();
                Debug.Log($"[{gameObject.name}] Added CustomerBehavior");
>>>>>>> Stashed changes
            }

            // 2. Add CustomerFoodRequest
            var foodRequest = GetComponent<CustomerFoodRequest>();
            if (foodRequest == null)
            {
                foodRequest = gameObject.AddComponent<CustomerFoodRequest>();
                Debug.Log($"[{gameObject.name}] Added CustomerFoodRequest");
            }

            // 3. Add AcceptFood
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
                // Try to find the WolfController in the project
                string[] guids = UnityEditor.AssetDatabase.FindAssets("WolfController t:AnimatorController");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    defaultController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
                    
                    if (defaultController != null)
                    {
                        animator.runtimeAnimatorController = defaultController;
                        Debug.Log($"[{gameObject.name}] Found and assigned WolfController");
                        
                        // Set up animator parameters
                        animator.SetBool("Walk", false);
                        animator.SetBool("Run", false);
                    }
                    else
                    {
                        Debug.LogError($"[{gameObject.name}] Could not load WolfController!");
                    }
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Could not find WolfController in project!");
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
