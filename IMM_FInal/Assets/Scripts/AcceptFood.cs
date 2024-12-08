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
            if (!other.gameObject.CompareTag("Food"))
            {
                Debug.Log($"[{gameObject.name}] Non-food object entered trigger: {other.gameObject.name} with tag {other.gameObject.tag}");
                return;
            }
            
            Debug.Log($"[{gameObject.name}] Food object entered trigger: {other.gameObject.name}");
            
            // Check if this customer is first in queue
            if (queueManager == null)
            {
                Debug.LogError($"[{gameObject.name}] Queue manager is null!");
                return;
            }

            if (wolfComponent == null)
            {
                Debug.LogError($"[{gameObject.name}] Wolf component is null!");
                return;
            }

            if (!queueManager.IsFirstInQueue(wolfComponent))
            {
                Debug.Log($"[{gameObject.name}] Not first in queue, ignoring food");
                return;
            }

            FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
            if (foodScript == null)
            {
                Debug.LogError($"[{gameObject.name}] FoodScript component not found on the Food GameObject.");
                return;
            }

            if (customerFoodRequest == null)
            {
                Debug.LogError($"[{gameObject.name}] CustomerFoodRequest is null!");
                return;
            }

            FoodType incomingFoodType = foodScript.FoodType;
            FoodType desiredFood = customerFoodRequest.GetDesiredFood();
            
            Debug.Log($"[{gameObject.name}] ========= FOOD COMPARISON =========");
            Debug.Log($"[{gameObject.name}] Desired Food Details:");
            Debug.Log($"  - Type: {desiredFood}");
            Debug.Log($"  - Enum Value: {(int)desiredFood}");
            Debug.Log($"  - String Value: {desiredFood.ToString()}");
            Debug.Log($"[{gameObject.name}] Incoming Food Details:");
            Debug.Log($"  - Type: {incomingFoodType}");
            Debug.Log($"  - Enum Value: {(int)incomingFoodType}");
            Debug.Log($"  - String Value: {incomingFoodType.ToString()}");
            
            // Try multiple comparison methods
            bool enumEqual = incomingFoodType == desiredFood;
            bool stringEqual = incomingFoodType.ToString().Equals(desiredFood.ToString(), System.StringComparison.OrdinalIgnoreCase);
            bool valueEqual = (int)incomingFoodType == (int)desiredFood;
            
            Debug.Log($"[{gameObject.name}] Comparison Results:");
            Debug.Log($"  - Enum Equality: {enumEqual}");
            Debug.Log($"  - String Equality: {stringEqual}");
            Debug.Log($"  - Value Equality: {valueEqual}");
            
            bool isCorrectFood = enumEqual || stringEqual || valueEqual;
            Debug.Log($"[{gameObject.name}] Final Result - Is correct food? {isCorrectFood}");

            if (isCorrectFood)
            {
                Debug.Log($"[{gameObject.name}] Correct food received, serving...");
                // Play happy animation when correct food enters trigger
                if (animationController != null)
                {
                    animationController.PlayHappyAnimation();
                    Debug.Log($"[{gameObject.name}] Triggering wave animation");
                }
                
                // Show response and serve food
                customerFoodRequest.ShowResponse(true);
                wolfComponent.ServeFood(incomingFoodType.ToString());
                
                // Destroy the food object
                Destroy(other.gameObject);
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Wrong food received, showing angry response");
                
                // Show angry response
                customerFoodRequest.ShowResponse(false);
                
                // Play angry animation if available
                if (animationController != null)
                {
                    animationController.PlayAngryAnimation();
                    Debug.Log($"[{gameObject.name}] Triggering angry animation");
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
