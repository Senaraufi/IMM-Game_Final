using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SojaExiles
{
    public class RegisterQueueManager : MonoBehaviour
    {
        private static RegisterQueueManager _instance;
        public static RegisterQueueManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RegisterQueueManager>();
                }
                return _instance;
            }
        }
        
        [SerializeField]
        private Transform registerPosition; // The main register position
        [SerializeField]
        private float spacingBetweenNPCs = 2f; // Distance between NPCs in the queue
        
        private Queue<animal_people_wolf_1> npcQueue = new Queue<animal_people_wolf_1>();
        private Dictionary<animal_people_wolf_1, Vector3> queuePositions = new Dictionary<animal_people_wolf_1, Vector3>();
        private List<animal_people_wolf_1> availableCustomers = new List<animal_people_wolf_1>();

        [SerializeField]
        private UnityEvent<animal_people_wolf_1> onCustomerRegistered;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                FindAllCustomers();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void FindAllCustomers()
        {
            GameObject[] customerObjects = GameObject.FindGameObjectsWithTag("Customer");
            foreach (GameObject customerObj in customerObjects)
            {
                if (customerObj == null) continue;
                
                animal_people_wolf_1 customer = customerObj.GetComponent<animal_people_wolf_1>();
                if (customer != null && !availableCustomers.Contains(customer))
                {
                    availableCustomers.Add(customer);
                    onCustomerRegistered?.Invoke(customer);
                }
            }
        }

        public void RegisterCustomer(animal_people_wolf_1 customer)
        {
            if (!availableCustomers.Contains(customer))
            {
                availableCustomers.Add(customer);
                onCustomerRegistered?.Invoke(customer);
            }
        }

        public Vector3 GetQueuePosition(animal_people_wolf_1 npc)
        {
            if (queuePositions.ContainsKey(npc))
            {
                return queuePositions[npc];
            }

            // Calculate new position based on queue length
            Vector3 position = registerPosition.position - (registerPosition.forward * spacingBetweenNPCs * npcQueue.Count);
            queuePositions[npc] = position;
            npcQueue.Enqueue(npc);
            
            return position;
        }

        public void RemoveFromQueue(animal_people_wolf_1 npc)
        {
            if (queuePositions.ContainsKey(npc))
            {
                queuePositions.Remove(npc);
                // Convert queue to list to remove specific NPC
                var tempList = new List<animal_people_wolf_1>(npcQueue);
                tempList.Remove(npc);
                npcQueue = new Queue<animal_people_wolf_1>(tempList);
                
                // Recalculate positions for remaining NPCs
                UpdateQueuePositions();
            }
        }

        private void UpdateQueuePositions()
        {
            int index = 0;
            queuePositions.Clear();
            
            foreach (var npc in npcQueue)
            {
                if (npc != null)
                {
                    Vector3 newPosition = registerPosition.position - (registerPosition.forward * spacingBetweenNPCs * index);
                    queuePositions[npc] = newPosition;
                    npc.UpdateQueuePosition(newPosition);
                    index++;
                }
            }
        }

        public bool IsFirstInQueue(animal_people_wolf_1 npc)
        {
            return npcQueue.Count > 0 && npcQueue.Peek() == npc;
        }

        public void TriggerAllCustomersToRegister()
        {
            foreach (var customer in availableCustomers.ToArray())
            {
                if (customer != null && !npcQueue.Contains(customer))
                {
                    customer.StartMovingToRegister();
                }
            }
        }
    }
}