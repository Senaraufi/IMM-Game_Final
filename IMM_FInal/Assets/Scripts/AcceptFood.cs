using UnityEngine;
using System.Collections;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerBehavior customerBehavior;
        private bool hasReceivedFood = false;

        void Awake()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            if (customerFoodRequest == null)
            {
                customerFoodRequest = gameObject.AddComponent<CustomerFoodRequest>();
            }
            
            customerBehavior = GetComponent<CustomerBehavior>();
            if (customerBehavior == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing CustomerBehavior component!");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasReceivedFood && customerFoodRequest != null)
            {
                if (!other.gameObject.CompareTag("Food"))
                {
                    return;
                }

                // Get food component
                FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    return;
                }

                ProcessFoodDelivery(foodScript);
            }
        }

        public bool AcceptFoodItem(FoodType foodType, GameObject foodObject)
        {
            if (hasReceivedFood || customerFoodRequest == null)
            {
                Debug.LogWarning("Cannot accept food - already received food or missing components");
                return false;
            }

            hasReceivedFood = true;

            // Compare with desired food
            bool isCorrectFood = foodType == customerFoodRequest.GetDesiredFood();
            Debug.Log($"[{gameObject.name}] Received {foodType}, wanted {customerFoodRequest.GetDesiredFood()}, correct: {isCorrectFood}");

            // Show response in the same text bubble
            customerFoodRequest.ShowResponse(isCorrectFood);

            // Destroy the food
            Destroy(foodObject);

            // Start the leave sequence after showing response
            StartCoroutine(LeaveAfterResponse());

            return isCorrectFood;
        }

        private IEnumerator LeaveAfterResponse()
        {
            // Wait for response to be visible
            yield return new WaitForSeconds(2f);

            // Make customer leave
            if (customerBehavior != null)
            {
                // Mark as served and update queue
                customerBehavior.ServeFood("Served");
                
                // Make them walk back to start
                customerBehavior.StartReturnToStart();
                
                Debug.Log($"[{gameObject.name}] Customer leaving after response");
            }
        }

        private void ProcessFoodDelivery(FoodScript foodScript)
        {
            AcceptFoodItem(foodScript.FoodType, foodScript.gameObject);
        }
    }
}
