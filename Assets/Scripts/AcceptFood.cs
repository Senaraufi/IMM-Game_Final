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

        private void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            wolfComponent = GetComponent<animal_people_wolf_1>();
            queueManager = RegisterQueueManager.Instance;
            
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

            if (queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] RegisterQueueManager not found!");
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
            if (customerFoodRequest == null || wolfComponent == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing required components!");
                return false;
            }
            
            // Check if this customer is first in queue
            if (queueManager == null || !queueManager.IsFirstInQueue(wolfComponent))
            {
                Debug.Log($"[{gameObject.name}] Not first in queue or queue manager missing, ignoring food");
                return false;
            }

            FoodType desiredFood = customerFoodRequest.GetDesiredFood();
            Debug.Log($"[{gameObject.name}] Detailed food comparison (AcceptFoodItem):");
            Debug.Log($"  - Desired food: {desiredFood} (type: {desiredFood.GetType()}, value: {(int)desiredFood})");
            Debug.Log($"  - Received food: {type} (type: {type.GetType()}, value: {(int)type})");
            Debug.Log($"  - Direct equality: {type == desiredFood}");
            Debug.Log($"  - String comparison: {type.ToString() == desiredFood.ToString()}");

            bool isCorrectFood = (type == desiredFood);
            Debug.Log($"[{gameObject.name}] Is correct food? {isCorrectFood}");
            
            if (isCorrectFood)
            {
                Debug.Log($"[{gameObject.name}] Correct food received, serving...");
                // Play happy animation when food is accepted
                if (animationController != null)
                {
                    animationController.PlayHappyAnimation();
                    Debug.Log($"[{gameObject.name}] Triggering wave animation");
                }
                
                if (heldFood != null)
                {
                    Destroy(heldFood);
                }
                
                // Show response and serve food
                customerFoodRequest.ShowResponse(true);
                wolfComponent.ServeFood(type.ToString());
                return true;
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Wrong food received, showing angry response");
                customerFoodRequest.ShowResponse(false);
                return false;
            }
        }
    }
}
