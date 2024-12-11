using UnityEngine;

namespace SojaExiles
{
    public class Trigger : MonoBehaviour
    {
        private bool hasTriggered = false;
        private RegisterQueueManager queueManager;

        void Start()
        {
            queueManager = RegisterQueueManager.Instance;
        }

        void OnTriggerEnter(Collider other)
        {
            // Check if the player has entered the trigger and it hasn't been triggered yet
            if (other.CompareTag("Player") && !hasTriggered && queueManager != null)
            {
                // Find all customers in the scene but only once
                var customers = FindObjectsOfType<CustomerBehavior>();
                foreach (var customer in customers)
                {
                    if (customer != null && customer.CompareTag("Customer"))
                    {
                        customer.StartMovingToRegister();
                    }
                }
                hasTriggered = true;
            }

            // Check if a customer has reached the counter
            if (other.CompareTag("Customer"))
            {
                var customerRequest = other.GetComponent<CustomerFoodRequest>();
                if (customerRequest != null)
                {
                    customerRequest.SetAtCounter(true);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            // Reset trigger when player exits
            if (other.CompareTag("Player"))
            {
                hasTriggered = false;
            }

            // When customer leaves the counter area
            if (other.CompareTag("Customer"))
            {
                var customerRequest = other.GetComponent<CustomerFoodRequest>();
                if (customerRequest != null)
                {
                    customerRequest.SetAtCounter(false);
                }
            }
        }
    }
}