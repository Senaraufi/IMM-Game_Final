using UnityEngine;
using UnityEditor;

namespace SojaExiles
{
    [RequireComponent(typeof(Animator))]
    public class CustomerSetup : MonoBehaviour
    {
        private Animator animator;
        private RuntimeAnimatorController defaultController;
        private RegisterQueueManager queueManager;
        private animal_people_wolf_1 wolfComponent;

        void Awake()
        {
            // Get the animator component
            animator = GetComponent<Animator>();
            
            // Try to find the animator controller if it's not set
            if (animator.runtimeAnimatorController == null)
            {
                // Try to find the controller in its original location
                defaultController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Supercyan Character Pack Animal People Sample/AnimatorControllers/FreeAnimations.controller");
                
                if (defaultController != null)
                {
                    animator.runtimeAnimatorController = defaultController;
                }
                else
                {
                    Debug.LogError("Could not find animator controller! Make sure it exists in: Assets/Supercyan Character Pack Animal People Sample/AnimatorControllers/FreeAnimations.controller");
                }
            }

            // Add CustomerAnimationController if it doesn't exist
            if (GetComponent<CustomerAnimationController>() == null)
            {
                gameObject.AddComponent<CustomerAnimationController>();
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

            // Get wolf component
            wolfComponent = GetComponent<animal_people_wolf_1>();
            if (wolfComponent == null)
            {
                Debug.LogError($"[{gameObject.name}] animal_people_wolf_1 component not found!");
                return;
            }

            // Register with queue manager
            if (CompareTag("Customer"))
            {
                Debug.Log($"[{gameObject.name}] Registering with queue manager");
                queueManager.RegisterCustomer(wolfComponent);
            }
        }
    }
}
