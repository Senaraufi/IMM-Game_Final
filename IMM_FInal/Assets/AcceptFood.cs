using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SojaExiles;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        [SerializeField]
        private TextHide textHide;

        [SerializeField]
        private FoodType acceptedFoodType = FoodType.pizza;

        private CustomerAnimationController animationController;

        private void Start()
        {
            animationController = GetComponent<CustomerAnimationController>();
            
            // Check if TextHide is assigned
            if (textHide == null)
            {
                Debug.LogError($"[{gameObject.name}] TextHide component not assigned in inspector!");
            }
            
            if (animationController == null)
            {
                Debug.LogError($"[{gameObject.name}] CustomerAnimationController not found!");
            }
        }

        private void ShowMessage(string message)
        {
            if (textHide != null)
            {
                textHide.ShowText(message);
            }
            Debug.Log($"[{gameObject.name}] {message}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Food"))
            {
                FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    Debug.LogError($"[{gameObject.name}] FoodScript component not found on the Food GameObject.");
                    return;
                }

                FoodType incomingFoodType = foodScript.FoodType;

                if (incomingFoodType != acceptedFoodType)
                {
                    ShowMessage("FUCK OFF");
                }
                else
                {
                    ShowMessage("Food accepted");
                    // Play happy animation when correct food enters trigger
                    if (animationController != null)
                    {
                        animationController.PlayHappyAnimation();
                        Debug.Log($"[{gameObject.name}] Triggering wave animation from OnTriggerEnter");
                    }
                }
            }
        }

        public bool AcceptFoodItem(FoodType type, GameObject heldFood)
        {
            if (type != acceptedFoodType)
            {
                ShowMessage("FUCK OFF");
                return false;
            }
            else
            {
                ShowMessage("Food accepted");
                
                // Play happy animation when food is accepted
                if (animationController != null)
                {
                    animationController.PlayHappyAnimation();
                    Debug.Log($"[{gameObject.name}] Triggering wave animation from AcceptFoodItem");
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Animation controller not found!");
                }
                
                if (heldFood != null)
                {
                    Destroy(heldFood);
                }
                return true;
            }
        }
    }
}
