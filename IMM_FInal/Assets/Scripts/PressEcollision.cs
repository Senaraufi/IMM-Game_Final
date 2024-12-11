using UnityEngine;
using TMPro;

namespace SojaExiles
{
    public class PressEcollision : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI pressE;
        
        private bool isPlayerInRange = false;
        private RegisterQueueManager queueManager;

        void Start()
        {
            if (pressE == null)
            {
                var textObj = GameObject.Find("PressEText");
                if (textObj != null)
                {
                    pressE = textObj.GetComponent<TextMeshProUGUI>();
                }
            }

            queueManager = RegisterQueueManager.Instance;
            if (queueManager == null)
            {
                Debug.LogError("RegisterQueueManager not found!");
            }

            // Make sure text is hidden at start
            if (pressE != null)
            {
                pressE.text = "";
                pressE.enabled = false;
            }
        }

        void Update()
        {
            if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
            {
                ServeCustomers();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && pressE != null)
            {
                isPlayerInRange = true;
                pressE.text = "Press E to serve customers";
                pressE.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && pressE != null)
            {
                isPlayerInRange = false;
                pressE.text = "";
                pressE.enabled = false;
            }
        }

        private void ServeCustomers()
        {
            var customers = FindObjectsOfType<CustomerBehavior>();
            foreach (var customer in customers)
            {
                if (customer != null && customer.CompareTag("Customer"))
                {
                    customer.StartMovingToRegister();
                }
            }
        }
    }
}
