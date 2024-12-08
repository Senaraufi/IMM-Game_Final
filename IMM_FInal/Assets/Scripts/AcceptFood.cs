using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SojaExiles;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerAnimationController animationController;
        private animal_people_wolf_1 wolfComponent;

        private void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            wolfComponent = GetComponent<animal_people_wolf_1>();
            
            if (customerFoodRequest == null)
            {
                Debug.LogError($"[{gameObject.name}] CustomerFoodRequest component not found!");
            }
            
            if (animationController == null)
            {
                Debug.LogError($"[{gameObject.name}] CustomerAnimationController not found!");
            }

            if (wolfComponent == null)
            {
                Debug.LogError($"[{gameObject.name}] animal_people_wolf_1 component not found!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Food"))
            {
                Debug.Log($"[{gameObject.name}] Food object entered trigger: {other.gameObject.name}");
                
                FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    Debug.LogError($"[{gameObject.name}] FoodScript component not found on the Food GameObject.");
                    return;
                }

                FoodType incomingFoodType = foodScript.FoodType;
                FoodType desiredFood = customerFoodRequest.GetDesiredFood();
                
                Debug.Log($"[{gameObject.name}] Food type comparison:");
                Debug.Log($"  - Desired food (enum value): {desiredFood} ({(int)desiredFood})");
                Debug.Log($"  - Received food (enum value): {incomingFoodType} ({(int)incomingFoodType})");
                
                bool isCorrectFood = incomingFoodType == desiredFood;
                Debug.Log($"[{gameObject.name}] Is correct food? {isCorrectFood}");
                
                customerFoodRequest.ShowResponse(isCorrectFood);
                
                if (isCorrectFood && wolfComponent != null)
                {
                    // Play happy animation when correct food enters trigger
                    if (animationController != null)
                    {
                        animationController.PlayHappyAnimation();
                        Debug.Log($"[{gameObject.name}] Triggering wave animation from OnTriggerEnter");
                    }
                    
                    // Serve food and trigger leaving sequence
                    wolfComponent.ServeFood(incomingFoodType.ToString());
                }
            }
        }

        public bool AcceptFoodItem(FoodType type, GameObject heldFood)
        {
            if (customerFoodRequest == null || wolfComponent == null) return false;

            bool isCorrectFood = (type == customerFoodRequest.GetDesiredFood());
            customerFoodRequest.ShowResponse(isCorrectFood);
            
            if (isCorrectFood)
            {
                // Play happy animation when food is accepted
                if (animationController != null)
                {
                    animationController.PlayHappyAnimation();
                    Debug.Log($"[{gameObject.name}] Triggering wave animation from AcceptFoodItem");
                }
                
                if (heldFood != null)
                {
                    Destroy(heldFood);
                }
                
                // Serve food and trigger leaving sequence
                wolfComponent.ServeFood(type.ToString());
                return true;
            }
            
            return false;
        }
    }
}
