using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SojaExiles
{
    public class RegisterQueueManager : MonoBehaviour
    {
        private static RegisterQueueManager _instance;
        public static RegisterQueueManager Instance => _instance;
        
        [SerializeField]
        private Transform registerPosition;
        
        [SerializeField]
        private float spacingBetweenNPCs = 1.5f;
        
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
                DontDestroyOnLoad(gameObject);
                FindAllCustomers();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void FindAllCustomers()
        {
            var customers = FindObjectsOfType<animal_people_wolf_1>();
            foreach (var customer in customers)
            {
                if (customer.CompareTag("Customer"))
                {
                    availableCustomers.Add(customer);
                }
            }
        }

        public void RegisterCustomer(animal_people_wolf_1 customer)
        {
            if (customer == null || !customer.CompareTag("Customer")) return;
            
            if (!npcQueue.Contains(customer))
            {
                npcQueue.Enqueue(customer);
                UpdateQueuePositions();
            }
        }

        public void CustomerLeaving(animal_people_wolf_1 customer)
        {
            if (queuePositions.ContainsKey(customer))
            {
                queuePositions.Remove(customer);
                var tempQueue = new Queue<animal_people_wolf_1>();
                while (npcQueue.Count > 0)
                {
                    var npc = npcQueue.Dequeue();
                    if (npc != customer)
                    {
                        tempQueue.Enqueue(npc);
                    }
                }
                npcQueue = tempQueue;
                UpdateQueuePositions();
            }
        }

        private void UpdateQueuePositions()
        {
            if (registerPosition == null) return;

            queuePositions.Clear();
            Vector3 basePosition = registerPosition.position;
            Vector3 forward = registerPosition.forward;
            
            int index = 0;
            foreach (var npc in npcQueue)
            {
                // Calculate position behind the register
                Vector3 queuePosition = basePosition + (-forward * spacingBetweenNPCs * index);
                queuePosition.y = basePosition.y; // Keep at ground level
                
                queuePositions[npc] = queuePosition;
                index++;
            }
        }

        public Vector3 GetPositionInQueue(animal_people_wolf_1 customer)
        {
            if (customer == null)
            {
                Debug.LogError("Trying to get position for null customer!");
                return Vector3.zero;
            }

            return queuePositions.TryGetValue(customer, out Vector3 position) ? position : customer.transform.position;
        }

        public bool IsFirstInQueue(animal_people_wolf_1 customer)
        {
            if (customer == null)
            {
                Debug.LogError("Checking if null customer is first in queue!");
                return false;
            }

            return npcQueue.Count > 0 && npcQueue.Peek() == customer;
        }

        public Vector3 GetStartPosition()
        {
            // Return a position where customers start from (entrance)
            return registerPosition.position + (registerPosition.right * 10f);
        }
    }
}