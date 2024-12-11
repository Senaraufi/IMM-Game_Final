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
        
        private Queue<MonoBehaviour> npcQueue = new Queue<MonoBehaviour>();
        private Dictionary<MonoBehaviour, Vector3> queuePositions = new Dictionary<MonoBehaviour, Vector3>();
        private List<MonoBehaviour> availableCustomers = new List<MonoBehaviour>();

        [SerializeField]
        private UnityEvent<MonoBehaviour> onCustomerRegistered;
        
        [SerializeField]
        private UnityEvent<MonoBehaviour> onCustomerLeaving;

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
            var customers = FindObjectsOfType<MonoBehaviour>();
            foreach (var customer in customers)
            {
                if (customer.CompareTag("Customer"))
                {
                    availableCustomers.Add(customer);
                }
            }
            Debug.Log($"Found {availableCustomers.Count} customers in the scene");
        }

        public void RegisterCustomer(MonoBehaviour customer)
        {
            if (!npcQueue.Contains(customer))
            {
                npcQueue.Enqueue(customer);
                onCustomerRegistered?.Invoke(customer);
                UpdateQueuePositions();
            }
        }

        public bool IsFirstInQueue(MonoBehaviour customer)
        {
            return npcQueue.Count > 0 && npcQueue.Peek() == customer;
        }

        public void CustomerLeaving(MonoBehaviour customer)
        {
            if (npcQueue.Contains(customer))
            {
                npcQueue = new Queue<MonoBehaviour>(npcQueue.Where(c => c != customer));
                onCustomerLeaving?.Invoke(customer);
                UpdateQueuePositions();
            }
        }

        public bool GetIsFirstInQueue(MonoBehaviour customer)
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

        public Vector3 GetQueuePosition(MonoBehaviour npc)
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