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
                    ShowMessage("Wrong order!");
                    if (animationController != null)
                    {
                        animationController.PlayAngryAnimation();
                    }
                }
                else
                {
                    ShowMessage("Thank you!");
                    if (animationController != null)
                    {
                        animationController.PlayHappyAnimation();
                        Debug.Log($"[{gameObject.name}] Playing happy animation for correct food");
                    }
                    Destroy(other.gameObject);
                }
            }
        }

        public bool AcceptFoodItem(FoodType type, GameObject heldFood)
        {
            if (type != acceptedFoodType)
            {
                ShowMessage("Wrong order!");
                if (animationController != null)
                {
                    animationController.PlayAngryAnimation();
                }
                return false;
            }
            
            ShowMessage("Thank you!");
            if (animationController != null)
            {
                animationController.PlayHappyAnimation();
                Debug.Log($"[{gameObject.name}] Playing happy animation for accepted food");
            }
            
            if (heldFood != null)
            {
                Destroy(heldFood);
            }
            return true;
        }
    }
}
