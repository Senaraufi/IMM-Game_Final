using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;

namespace SojaExiles
{
    public class RegisterQueueManager : MonoBehaviour
    {
        private static RegisterQueueManager _instance;
        public static RegisterQueueManager Instance => _instance;
        
        [SerializeField]
        private Transform registerPosition;
        
        [SerializeField]
        private float spacing = 1.5f;
        
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
            }
        }

        public bool IsFirstInQueue(animal_people_wolf_1 customer)
        {
            return npcQueue.Count > 0 && npcQueue.Peek() == customer;
        }

        public void CustomerLeaving(animal_people_wolf_1 customer)
        {
            if (npcQueue.Contains(customer))
            {
                npcQueue = new Queue<animal_people_wolf_1>(npcQueue.Where(c => c != customer));
                onCustomerLeaving?.Invoke(customer);
                UpdateQueuePositions();
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
                return;
            }

            // Clear old positions
            queuePositions.Clear();

            // Calculate new positions
            Vector3 basePosition = registerPosition.position;
            int index = 0;
            foreach (var npc in npcQueue)
            {
                Vector3 queuePosition = basePosition + (Vector3.back * spacing * index);
                queuePositions[npc] = queuePosition;
                index++;
            }
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