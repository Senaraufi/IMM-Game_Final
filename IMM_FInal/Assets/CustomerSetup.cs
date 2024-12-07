using UnityEngine;
using UnityEditor;

namespace SojaExiles
{
    [RequireComponent(typeof(Animator))]
    public class CustomerSetup : MonoBehaviour
    {
        private Animator animator;
        private RuntimeAnimatorController defaultController;

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
    }
}
