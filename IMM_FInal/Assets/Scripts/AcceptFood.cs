using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerBehavior customerBehavior;
        private bool isProcessingDelivery = false;

        void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            customerBehavior = GetComponent<CustomerBehavior>();
            
            if (customerFoodRequest == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing CustomerFoodRequest component!");
                customerFoodRequest = gameObject.AddComponent<CustomerFoodRequest>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isProcessingDelivery && customerFoodRequest != null)
            {
                if (!other.gameObject.CompareTag("Food"))
                {
                    return;
                }

                // Get components
                FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    return;
                }

                // Get food types
                FoodType incomingFood = foodScript.FoodType;
                FoodType wantedFood = customerFoodRequest.GetDesiredFood();

                // Simple direct comparison
                bool isCorrectFood = incomingFood == wantedFood;
                
                if (isCorrectFood)
                {
                    // Show response only if customer is first in queue
                    customerFoodRequest.ShowResponse(true);
                    
                    // Play animation
                    if (customerBehavior != null)
                    {
                        customerBehavior.PlayHappyAnimation();
                    }
                    
                    // Serve and cleanup
                    if (customerBehavior != null)
                    {
                        customerBehavior.ServeFood(incomingFood.ToString());
                    }
                    
                    // Make sure to clear the reference before destroying
                    if (foodScript != null)
                    {
                        Destroy(other.gameObject);
                    }
                }
                else
                {
                    // Show response only if customer is first in queue
                    customerFoodRequest.ShowResponse(false);
                    
                    // Play animation
                    if (customerBehavior != null)
                    {
                        customerBehavior.PlayAngryAnimation();
                    }

                    // Make the customer leave even with wrong food
                    if (customerBehavior != null)
                    {
                        customerBehavior.ServeFood("WrongFood");
                    }
                    Destroy(other.gameObject);
                }
            }
        }

        public bool AcceptFoodItem(FoodType foodType, GameObject foodObject)
        {
            if (isProcessingDelivery || customerFoodRequest == null)
            {
                Debug.LogWarning("Cannot accept food - already processing or missing components");
                return false;
            }

            Debug.Log($"Customer {gameObject.name} accepting food: {foodType}");
            isProcessingDelivery = true;

            // Compare with desired food
            bool isCorrectFood = foodType == customerFoodRequest.GetDesiredFood();
            Debug.Log($"Is correct food? {isCorrectFood}. Customer wanted: {customerFoodRequest.GetDesiredFood()}");
            
            // Show response
            customerFoodRequest.ShowResponse(isCorrectFood);
            
            // Start coroutine to process delivery
            StartCoroutine(ProcessDeliveryComplete(foodObject, isCorrectFood));
            return isCorrectFood;
        }

        private IEnumerator ProcessDeliveryComplete(GameObject foodObject, bool wasCorrectOrder)
        {
            Debug.Log($"Processing delivery completion for {gameObject.name}");
            
            // Wait for response to be visible
            yield return new WaitForSeconds(2f);
            
            // Destroy the food object
            if (foodObject != null)
                Destroy(foodObject);

            // Wait a bit more before leaving
            yield return new WaitForSeconds(1f);
            
            // Make customer leave
            if (customerBehavior != null)
            {
                Debug.Log($"Customer {gameObject.name} is leaving");
                customerBehavior.StartReturnToStart();
            }
            
            isProcessingDelivery = false;
        }
    }
}
