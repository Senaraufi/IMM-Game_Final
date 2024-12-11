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
        
        private Queue<CustomerBehavior> npcQueue = new Queue<CustomerBehavior>();
        private Dictionary<CustomerBehavior, Vector3> queuePositions = new Dictionary<CustomerBehavior, Vector3>();
        private List<CustomerBehavior> availableCustomers = new List<CustomerBehavior>();

        [SerializeField]
        private UnityEvent<CustomerBehavior> onCustomerRegistered;
        
        [SerializeField]
        private UnityEvent<CustomerBehavior> onCustomerLeaving;

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
            var customers = FindObjectsOfType<CustomerBehavior>();
            foreach (var customer in customers)
            {
                if (customer.CompareTag("Customer"))
                {
                    availableCustomers.Add(customer);
                }
            }
            Debug.Log($"Found {availableCustomers.Count} customers in the scene");
        }

        public void RegisterCustomer(CustomerBehavior customer)
        {
            if (!npcQueue.Contains(customer))
            {
                npcQueue.Enqueue(customer);
                onCustomerRegistered?.Invoke(customer);
                UpdateQueuePositions();
            }
        }

        public bool IsFirstInQueue(CustomerBehavior customer)
        {
            return npcQueue.Count > 0 && npcQueue.Peek() == customer;
        }

        public void CustomerLeaving(CustomerBehavior customer)
        {
            if (npcQueue.Contains(customer))
            {
                npcQueue = new Queue<CustomerBehavior>(npcQueue.Where(c => c != customer));
                onCustomerLeaving?.Invoke(customer);
                UpdateQueuePositions();
            }
        }

        public bool GetIsFirstInQueue(CustomerBehavior customer)
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

        public Vector3 GetQueuePosition(CustomerBehavior npc)
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