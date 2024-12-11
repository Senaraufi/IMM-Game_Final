using System.Collections;
using UnityEngine;

namespace SojaExiles
{
    public class AcceptFood : MonoBehaviour
    {
        private CustomerFoodRequest customerFoodRequest;
        private CustomerAnimationController animationController;
        private animal_people_wolf_1 wolfComponent;
        private bool isProcessingDelivery = false;

        void Start()
        {
            customerFoodRequest = GetComponent<CustomerFoodRequest>();
            animationController = GetComponent<CustomerAnimationController>();
            wolfComponent = GetComponent<animal_people_wolf_1>();
            
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
            if (wolfComponent != null)
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
