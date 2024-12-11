using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerAnimationController animationController;
        private MonoBehaviour customerComponent;
        private RegisterQueueManager queueManager;
        private bool isProcessingDelivery = false;

        void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            customerComponent = GetComponent<animal_people_wolf_1>();
            queueManager = RegisterQueueManager.Instance;
            
            if (customerFoodRequest == null)
            {
                return;
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

                // Queue check
                if (queueManager == null || customerComponent == null)
                {
                    return;
                }

                if (!queueManager.IsFirstInQueue(customerComponent))
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
                    if (queueManager.IsFirstInQueue(customerComponent))
                    {
                        customerFoodRequest.ShowResponse(true);
                    }
                    
                    // Play animation
                    if (animationController != null)
                    {
                        animationController.PlayHappyAnimation();
                    }
                    
                    // Serve and cleanup
                    if (customerComponent is animal_people_wolf_1 wolfComponent)
                    {
                        wolfComponent.ServeFood(incomingFood.ToString());
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
                    if (queueManager.IsFirstInQueue(customerComponent))
                    {
                        customerFoodRequest.ShowResponse(false);
                    }
                    
                    // Play animation
                    if (animationController != null)
                    {
                        animationController.PlayAngryAnimation();
                    }

                    // Make the customer leave even with wrong food
                    if (customerComponent is animal_people_wolf_1 wolfComponent)
                    {
                        wolfComponent.ServeFood("WrongFood");
                    }
                    Destroy(other.gameObject);
                }
            }
        }

        public bool AcceptFoodItem(FoodType foodType, GameObject foodObject)
        {
            if (isProcessingDelivery || customerFoodRequest == null)
                return false;

            isProcessingDelivery = true;

            // Compare with desired food
            bool isCorrectFood = (foodType == customerFoodRequest.GetDesiredFood());
            
            // Show response
            customerFoodRequest.ShowResponse(isCorrectFood);

            // Handle animations if available
            if (animationController != null)
            {
                if (isCorrectFood)
                    animationController.PlayHappyAnimation();
                else
                    animationController.PlayAngryAnimation();
            }

            // Handle wolf component if available
            if (customerComponent is animal_people_wolf_1 wolfComponent)
            {
                wolfComponent.ServeFood(foodType.ToString());
            }

            // Clean up
            if (foodObject != null)
                Destroy(foodObject);

            StartCoroutine(FinishDelivery());
            return isCorrectFood;
        }

        private IEnumerator FinishDelivery()
        {
            yield return new WaitForSeconds(2f);
            isProcessingDelivery = false;
        }
    }
}
