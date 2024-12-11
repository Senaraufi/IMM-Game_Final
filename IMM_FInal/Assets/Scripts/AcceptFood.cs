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
                if (other.CompareTag("Food"))
                {
                    var foodScript = other.GetComponent<FoodScript>();
                    if (foodScript != null)
                    {
                        AcceptFoodItem(foodScript.FoodType, other.gameObject);
                    }
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
            bool isCorrectFood = (foodType == customerFoodRequest.GetDesiredFood());
            Debug.Log($"Is correct food? {isCorrectFood}. Customer wanted: {customerFoodRequest.GetDesiredFood()}");
            
            // Show response first
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
            {
                Destroy(foodObject);
            }

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
