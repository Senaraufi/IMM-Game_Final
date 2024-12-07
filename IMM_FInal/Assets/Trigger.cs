using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class Trigger : MonoBehaviour
    {
        public animal_people_wolf_1 npcScript; // Reference to the NPC's script

        void OnTriggerEnter(Collider other)
        {
            // Check if the player has entered the trigger
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player entered KitchenTrigger, triggering NPC movement.");
                if (npcScript != null)
                {
                    npcScript.StartMovingToRegister(); // Call the NPC's method to start moving
                }
                else
                {
                    Debug.LogError("NPC script is not assigned in KitchenTriggerActivator!");
                }
            }
        }
    }
}