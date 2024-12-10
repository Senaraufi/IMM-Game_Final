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
        
        [SerializeField]
        private UnityEvent<animal_people_wolf_1> onCustomerLeaving;

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
            Debug.Log($"Found {availableCustomers.Count} customers in the scene");
        }

        public void RegisterCustomer(animal_people_wolf_1 customer)
        {
            if (!npcQueue.Contains(customer))
            {
                npcQueue.Enqueue(customer);
                onCustomerRegistered?.Invoke(customer);
                UpdateQueuePositions();
                Debug.Log($"Customer {customer.name} registered in queue. Queue size: {npcQueue.Count}");
            }
        }

        public bool IsFirstInQueue(animal_people_wolf_1 customer)
        {
            return npcQueue.Count > 0 && npcQueue.Peek() == customer;
        }

        public void CustomerLeaving(animal_people_wolf_1 customer)
        {
            if (npcQueue.Count > 0 && npcQueue.Peek() == customer)
            {
                npcQueue.Dequeue();
                onCustomerLeaving?.Invoke(customer);
                UpdateQueuePositions();
                Debug.Log($"Customer {customer.name} left the queue. Queue size: {npcQueue.Count}");
            }
        }

        public bool GetIsFirstInQueue(animal_people_wolf_1 customer)
        {
            return IsFirstInQueue(customer);
        }

        private void UpdateQueuePositions()
        {
            if (registerPosition == null)
            {
                Debug.LogError("Register position is not set!");
                return;
            }

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
            
            Debug.Log($"Updated queue positions for {queuePositions.Count} customers");
        }

        public Vector3 GetQueuePosition(animal_people_wolf_1 npc)
        {
            if (queuePositions.TryGetValue(npc, out Vector3 position))
            {
                return position;
            }
            
            // Return a position where customers start from (entrance)
            return registerPosition != null 
                ? registerPosition.position + (registerPosition.right * 10f) 
                : npc.transform.position;
        }
    }
}