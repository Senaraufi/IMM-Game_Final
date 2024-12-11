using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerAnimationController animationController;
        private CustomerBehavior customerBehavior;
        private RegisterQueueManager queueManager;
        private bool isProcessingDelivery = false;

        void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            customerBehavior = GetComponent<CustomerBehavior>();
            queueManager = RegisterQueueManager.Instance;
            
            if (customerFoodRequest == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing CustomerFoodRequest component!");
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
            if (isProcessingDelivery || customerFoodRequest == null || customerBehavior == null)
                return false;

            isProcessingDelivery = true;
            
            // Compare with desired food
            bool isCorrectFood = (foodType == customerFoodRequest.GetDesiredFood());
            
            // Show response first
            customerFoodRequest.ShowResponse(isCorrectFood);
            
            // Then serve food and play animation
            customerBehavior.ServeFood(foodType.ToString());
            
            if (isCorrectFood)
            {
                customerBehavior.PlayHappyAnimation();
            }
            else
            {
                customerBehavior.PlayAngryAnimation();
            }

            // Destroy the food object
            if (foodObject != null)
            {
                Destroy(foodObject);
            }

            StartCoroutine(ProcessDeliveryComplete());
            return isCorrectFood;
        }

        private IEnumerator ProcessDeliveryComplete()
        {
            yield return new WaitForSeconds(3f); // Wait longer for response animation
            isProcessingDelivery = false;
            
            if (customerBehavior != null)
            {
                customerBehavior.StartReturnToStart();
            }
        }
    }
}
