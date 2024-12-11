using UnityEngine;

namespace SojaExiles
{
    public class Trigger : MonoBehaviour
    {
        private bool hasTriggered = false;
        private RegisterQueueManager queueManager;

        void Start()
        {
            queueManager = RegisterQueueManager.Instance;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !hasTriggered && queueManager != null)
            {
                hasTriggered = true;
                // Find all NPCs and make them start moving to register
                var npcs = FindObjectsOfType<animal_people_wolf_1>();
                foreach (var npc in npcs)
                {
                    if (npc != null && npc.CompareTag("Customer"))
                    {
                        npc.StartMovingToRegister();
                    }
                }
                Debug.Log("Player entered trigger area, starting queue process");
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Reset trigger when player leaves
            if (other.CompareTag("Player"))
            {
                hasTriggered = false;
            }
        }
    }
}