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
            
            // Log debug info about NPCs
            var npcs = FindObjectsOfType<animal_people_wolf_1>();
            Debug.Log($"Found {npcs.Length} NPCs in the scene");
            foreach (var npc in npcs)
            {
                Debug.Log($"NPC: {npc.name}, Tag: {npc.tag}, HasNavMesh: {npc.GetComponent<UnityEngine.AI.NavMeshAgent>() != null}");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Check if the player has entered the trigger and it hasn't been triggered yet
            if (other.CompareTag("Player") && !hasTriggered && queueManager != null)
            {
                hasTriggered = true;
                queueManager.StartQueueProcessing();
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