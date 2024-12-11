using UnityEngine;

namespace SojaExiles
{
    public class Trigger : MonoBehaviour
    {
        private bool hasTriggered = false;

        void Start()
        {
            // Check for NPCs at start
            var customers = FindObjectsOfType<CustomerBehavior>();
            Debug.Log($"Found {customers.Length} customers in the scene");
            foreach (var customer in customers)
            {
                Debug.Log($"Customer: {customer.name}, Tag: {customer.tag}, HasNavMesh: {customer.GetComponent<UnityEngine.AI.NavMeshAgent>() != null}");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Trigger entered by: {other.name} with tag: {other.tag}");
            
            // Check if the player has entered the trigger and it hasn't been triggered yet
            if (other.CompareTag("Player") && !hasTriggered)
            {
                Debug.Log("Player entered KitchenTrigger, triggering customer movement.");
                
                // Find all customers in the scene
                var customers = FindObjectsOfType<CustomerBehavior>();
                Debug.Log($"Found {customers.Length} customers to move");
                
                foreach (var customer in customers)
                {
                    if (customer != null && customer.CompareTag("Customer"))
                    {
                        Debug.Log($"Starting movement for customer: {customer.name}");
                        customer.StartMovingToRegister();
                    }
                }
                
                hasTriggered = true;
            }

            // Check if a customer has reached the counter
            if (other.CompareTag("Customer"))
            {
                Debug.Log($"Customer entered counter area: {other.name}");
                var customerRequest = other.GetComponent<CustomerFoodRequest>();
                if (customerRequest != null)
                {
                    Debug.Log($"Found CustomerFoodRequest component on {other.name}, showing text");
                    customerRequest.SetAtCounter(true);
                }
                else
                {
                    Debug.LogError($"CustomerFoodRequest component missing on {other.name}!");
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            Debug.Log($"Trigger exited by: {other.name}");
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