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
        private RegisterQueueManager queueManager;
        private bool isProcessingDelivery = false;

        void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            wolfComponent = GetComponent<animal_people_wolf_1>();
            queueManager = RegisterQueueManager.Instance;
            
            if (customerFoodRequest == null || wolfComponent == null || queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing required components!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[{gameObject.name}] OnTriggerEnter with: {other.gameObject.name}");
            
            if (!other.gameObject.CompareTag("Food"))
            {
                Debug.Log($"[{gameObject.name}] Not a food object, ignoring");
                return;
            }

            // Get components
            FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
            if (foodScript == null)
            {
                Debug.LogError($"[{gameObject.name}] No FoodScript on food object!");
                return;
            }

            // Queue check
            if (queueManager == null || wolfComponent == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing queue manager or wolf component!");
                return;
            }

            if (!queueManager.IsFirstInQueue(wolfComponent))
            {
                Debug.Log($"[{gameObject.name}] Not first in queue, ignoring food");
                return;
            }

            // Get food types
            FoodType incomingFood = foodScript.FoodType;
            FoodType wantedFood = customerFoodRequest.GetDesiredFood();

            // Simple direct comparison
            bool isCorrectFood = incomingFood == wantedFood;
            
            Debug.Log($"[{gameObject.name}] Food comparison:");
            Debug.Log($"  Wanted: {wantedFood} ({(int)wantedFood})");
            Debug.Log($"  Got: {incomingFood} ({(int)incomingFood})");
            Debug.Log($"  Match? {isCorrectFood}");

            if (isCorrectFood)
            {
                Debug.Log($"[{gameObject.name}] Correct food! Serving...");
                
                // Show response first
                customerFoodRequest.ShowResponse(true);
                
                // Play animation
                if (animationController != null)
                {
                    animationController.PlayHappyAnimation();
                }
                
                // Serve and cleanup
                wolfComponent.ServeFood(incomingFood.ToString());
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Wrong food!");
                
                // Show response first
                customerFoodRequest.ShowResponse(false);
                
                // Play animation
                if (animationController != null)
                {
                    animationController.PlayAngryAnimation();
                }
            }
        }

        public bool AcceptFoodItem(FoodType type, GameObject heldFood)
        {
            if (isProcessingDelivery)
            {
                Debug.LogWarning($"[{gameObject.name}] Already processing a delivery!");
                return false;
            }

            if (!queueManager.IsFirstInQueue(wolfComponent))
            {
                Debug.Log($"[{gameObject.name}] Not first in queue, ignoring food");
                return false;
            }

            isProcessingDelivery = true;
            try
            {
                FoodType desiredFood = customerFoodRequest.GetDesiredFood();
                
                Debug.Log($"[{gameObject.name}] Food comparison:");
                Debug.Log($"  Wanted: {desiredFood} ({(int)desiredFood})");
                Debug.Log($"  Got: {type} ({(int)type})");
                Debug.Log($"  Match? {type == desiredFood}");

                bool isCorrectFood = (type == desiredFood);
                
                if (isCorrectFood)
                {
                    customerFoodRequest.ShowResponse(true);
                    if (animationController != null)
                    {
                        animationController.PlayHappyAnimation();
                    }
                    wolfComponent.ServeFood(type.ToString());
                    return true;
                }
                else
                {
                    customerFoodRequest.ShowResponse(false);
                    if (animationController != null)
                    {
                        animationController.PlayAngryAnimation();
                    }
                    return false;
                }
            }
            finally
            {
                isProcessingDelivery = false;
            }
        }
    }
}
