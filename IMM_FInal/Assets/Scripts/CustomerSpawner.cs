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
        public float spawnInterval = 10f;
        
        [SerializeField]
        private int maxCustomers = 5;

        private List<animal_people_wolf_1> spawnedCustomers = new List<animal_people_wolf_1>();
        private RegisterQueueManager queueManager;
        private bool isSpawning = true;
        private int currentWaitingSpot = 0;

        void Awake()
        {
            // If no customer prefab is assigned, try to load it
            if (customerPrefab == null)
            {
                customerPrefab = Resources.Load<GameObject>("animal_people_wolf_1");
                if (customerPrefab == null)
                {
                    Debug.LogError("Customer prefab not found! Please assign it in the inspector.");
                }
            }
        }

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

        private void SetupCustomerComponents(GameObject customer)
        {
            // Add required components if they don't exist
            if (!customer.GetComponent<Animator>())
            {
                customer.AddComponent<Animator>();
            }

            if (!customer.GetComponent<animal_people_wolf_1>())
            {
                customer.AddComponent<animal_people_wolf_1>();
            }

            if (!customer.GetComponent<CustomerAnimationController>())
            {
                customer.AddComponent<CustomerAnimationController>();
            }

            if (!customer.GetComponent<CustomerFoodRequest>())
            {
                customer.AddComponent<CustomerFoodRequest>();
            }

            if (!customer.GetComponent<AcceptFood>())
            {
                customer.AddComponent<AcceptFood>();
            }

            // Set the customer tag
            customer.tag = "Customer";

            // Try to load and set the animator controller
            var animator = customer.GetComponent<Animator>();
            if (animator && animator.runtimeAnimatorController == null)
            {
                RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>("FreeAnimations");
                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }
        }

        private GameObject SpawnCustomer()
        {
            if (customerPrefab == null) return null;

            Vector3 spawnPosition = transform.position;
            GameObject customer = Instantiate(customerPrefab, spawnPosition, Quaternion.identity);
            
            SetupCustomerComponents(customer);
            
            return customer;
        }

        IEnumerator SpawnCustomersRoutine()
        {
            while (isSpawning)
            {
                if (spawnedCustomers.Count < maxCustomers)
                {
                    GameObject customerObj = SpawnCustomer();
                    if (customerObj != null)
                    {
                        PositionNPC(customerObj);
                        spawnedCustomers.Add(customerObj.GetComponent<animal_people_wolf_1>());
                    }
                }
                yield return new WaitForSeconds(spawnInterval);
            }
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
