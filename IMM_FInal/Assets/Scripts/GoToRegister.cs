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

            var npcs = FindObjectsOfType<CustomerBehavior>();
            foreach (var npc in npcs)
            {
                if (queueManager.IsFirstInQueue(npc))
                {
                    var foodRequest = npc.GetComponent<CustomerFoodRequest>();
                    if (foodRequest != null)
                    {
                        FoodType requestedFood = foodRequest.GetDesiredFood();
                        Debug.Log($"Customer wants: {requestedFood}, Serving: {foodToServe}");
                        
                        FoodType servingFood;
                        if (System.Enum.TryParse(foodToServe, true, out servingFood))
                        {
                            bool isCorrectOrder = servingFood == requestedFood;
                            
                            // Always show response and serve food
                            foodRequest.ShowResponse(isCorrectOrder);
                            npc.ServeFood(foodToServe);
                            onServeCustomer?.Invoke();
                        }
                        else
                        {
                            Debug.LogError($"Could not parse food type: {foodToServe}");
                        }
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
