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

            var npcs = FindObjectsOfType<animal_people_wolf_1>();
            foreach (var npc in npcs)
            {
                if (queueManager.IsFirstInQueue(npc))
                {
                    npc.ServeFood(foodToServe);
                    onServeCustomer?.Invoke();
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
