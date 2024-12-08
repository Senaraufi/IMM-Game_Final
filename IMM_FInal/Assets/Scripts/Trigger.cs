using UnityEngine;

namespace SojaExiles
{
    public class Trigger : MonoBehaviour
    {
        private bool hasTriggered = false;

        void Start()
        {
            // Check for NPCs at start
            var npcs = FindObjectsOfType<animal_people_wolf_1>();
            Debug.Log($"Found {npcs.Length} NPCs in the scene");
            foreach (var npc in npcs)
            {
                Debug.Log($"NPC: {npc.name}, Tag: {npc.tag}, HasNavMesh: {npc.GetComponent<UnityEngine.AI.NavMeshAgent>() != null}");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Trigger entered by: {other.name} with tag: {other.tag}");
            
            // Check if the player has entered the trigger and it hasn't been triggered yet
            if (other.CompareTag("Player") && !hasTriggered)
            {
                Debug.Log("Player entered KitchenTrigger, triggering NPC movement.");
                
                // Find all NPCs in the scene
                var npcs = FindObjectsOfType<animal_people_wolf_1>();
                Debug.Log($"Found {npcs.Length} NPCs to move");
                
                foreach (var npc in npcs)
                {
                    if (npc != null)
                    {
                        Debug.Log($"Processing NPC: {npc.name}, Tag: {npc.tag}");
                        if (npc.CompareTag("Customer"))
                        {
                            Debug.Log($"Starting movement for NPC: {npc.name}");
                            npc.StartMovingToRegister();
                        }
                        else
                        {
                            Debug.Log($"NPC {npc.name} doesn't have Customer tag");
                        }
                    }
                }
                
                hasTriggered = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            Debug.Log($"Trigger exited by: {other.name}");
            // Reset trigger when player exits
            if (other.CompareTag("Player"))
            {
                hasTriggered = false;
            }
        }
    }
}