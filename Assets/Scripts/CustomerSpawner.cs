using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SojaExiles
{
    public class CustomerSpawner : MonoBehaviour
    {
        [SerializeField]
        private Transform[] waitingPositions; // Array of positions where NPCs will wait
        
        [SerializeField]
        private GameObject customerPrefab;
        
        [SerializeField]
        private float spawnInterval = 10f;
        
        [SerializeField]
        private int maxCustomers = 5;

        private List<animal_people_wolf_1> spawnedCustomers = new List<animal_people_wolf_1>();
        private RegisterQueueManager queueManager;
        private bool isSpawning = true;
        private int currentWaitingSpot = 0;

        void Start()
        {
            queueManager = RegisterQueueManager.Instance;
            if (queueManager == null)
            {
                Debug.LogError("RegisterQueueManager not found!");
                return;
            }

            // Position existing NPCs
            PositionExistingNPCs();

            StartCoroutine(SpawnCustomersRoutine());
        }

        private void PositionExistingNPCs()
        {
            var existingNPCs = FindObjectsOfType<animal_people_wolf_1>();
            foreach (var npc in existingNPCs)
            {
                if (npc != null && npc.CompareTag("Customer"))
                {
                    PositionNPC(npc.gameObject);
                    spawnedCustomers.Add(npc);
                }
            }
        }

        private void PositionNPC(GameObject customerObj)
        {
            if (currentWaitingSpot < waitingPositions.Length)
            {
                customerObj.transform.position = waitingPositions[currentWaitingSpot].position;
                customerObj.transform.rotation = waitingPositions[currentWaitingSpot].rotation;
                currentWaitingSpot++;
            }
            else
            {
                Debug.LogWarning("No more waiting positions available!");
            }
        }

        IEnumerator SpawnCustomersRoutine()
        {
            while (isSpawning)
            {
                if (spawnedCustomers.Count < maxCustomers)
                {
                    SpawnCustomer();
                }
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnCustomer()
        {
            if (customerPrefab == null || currentWaitingSpot >= waitingPositions.Length)
            {
                Debug.LogError("Cannot spawn customer: missing prefab or no waiting positions left");
                return;
            }

            // Spawn the customer at the next waiting position
            GameObject customerObj = Instantiate(customerPrefab, waitingPositions[currentWaitingSpot].position, 
                                              waitingPositions[currentWaitingSpot].rotation);
            
            // Get the customer script component
            var customer = customerObj.GetComponent<animal_people_wolf_1>();
            if (customer == null)
            {
                Debug.LogError("Spawned customer prefab doesn't have animal_people_wolf_1 component!");
                Destroy(customerObj);
                return;
            }

            // Set up the customer
            customerObj.tag = "Customer";
            spawnedCustomers.Add(customer);
            currentWaitingSpot++;
        }

        public void RemoveCustomer(animal_people_wolf_1 customer)
        {
            if (spawnedCustomers.Contains(customer))
            {
                spawnedCustomers.Remove(customer);
                queueManager.CustomerLeaving(customer);
                currentWaitingSpot--; // Free up a waiting spot
                currentWaitingSpot = Mathf.Max(0, currentWaitingSpot); // Ensure it doesn't go below 0
            }
        }

        void OnDestroy()
        {
            isSpawning = false;
        }
    }
}
