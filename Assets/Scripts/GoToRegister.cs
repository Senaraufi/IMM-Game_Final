using UnityEngine;
using UnityEngine.Events;

namespace SojaExiles
{
    public class GoToRegister : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onServeCustomer;
        
        [SerializeField]
        private string foodToServe = "burger"; // Default food type

        private RegisterQueueManager queueManager;

        void Start()
        {
            queueManager = RegisterQueueManager.Instance;
            if (queueManager == null)
            {
                Debug.LogError("RegisterQueueManager not found in the scene!");
            } 
        }

        // This method will be called by ButtonHandler's UnityEvent
        public void OnButtonPress()
        {
            ServeCurrentCustomer();
        }

        private void ServeCurrentCustomer()
        {
            if (queueManager == null)
            {
                Debug.LogError("RegisterQueueManager is not initialized!");
                return;
            }

            Debug.Log("Attempting to serve customer...");
            var npcs = FindObjectsOfType<animal_people_wolf_1>();
            foreach (var npc in npcs)
            {
                if (queueManager.IsFirstInQueue(npc))
                {
                    Debug.Log($"Found first customer in queue: {npc.name}");
                    // Get the customer's food request
                    var foodRequest = npc.GetComponent<CustomerFoodRequest>();
                    if (foodRequest != null)
                    {
                        FoodType requestedFood = foodRequest.GetDesiredFood();
                        Debug.Log($"Customer wants: {requestedFood}, Trying to serve: {foodToServe}");
                        
                        // Convert string to FoodType
                        FoodType servingFood;
                        if (System.Enum.TryParse(foodToServe, true, out servingFood))
                        {
                            bool isCorrectOrder = servingFood == requestedFood;
                            Debug.Log($"Order is correct: {isCorrectOrder}");
                            
                            // Show the appropriate response
                            foodRequest.ShowResponse(isCorrectOrder);
                            
                            if (isCorrectOrder)
                            {
                                Debug.Log("Serving correct food to customer");
                                // Only serve food and trigger events if the order was correct
                                npc.ServeFood(foodToServe);
                                onServeCustomer?.Invoke();
                            }
                        }
                        else
                        {
                            Debug.LogError($"Could not parse food type: {foodToServe}");
                        }
                    }
                    else
                    {
                        Debug.LogError($"CustomerFoodRequest component missing on {npc.name}!");
                    }
                    break;
                }
            }
        }

        // Method to change what food will be served
        public void SetFoodType(string foodType)
        {
            foodToServe = foodType;
        }
    }
}
