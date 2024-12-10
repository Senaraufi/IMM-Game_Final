using UnityEngine;

namespace SojaExiles
{
    public class CustomerPosition : MonoBehaviour
    {
        [Header("Position Settings")]
        [SerializeField] private Vector3 counterOffset = new Vector3(0f, 0f, 2f); // Offset from kitchen counter
        [SerializeField] private float interactionRange = 3f; // Range for food interaction
        
        [Header("References")]
        [SerializeField] private Transform kitchenCounter; // Reference to kitchen counter
        
        private void Start()
        {
            // Find kitchen counter if not assigned
            if (kitchenCounter == null)
            {
                GameObject counter = GameObject.FindGameObjectWithTag("KitchenTrigger");
                if (counter != null)
                {
                    kitchenCounter = counter.transform;
                }
                else
                {
                    Debug.LogError($"[{gameObject.name}] Kitchen counter not found! Make sure it has the 'KitchenTrigger' tag.");
                    return;
                }
            }
            
            // Position the customer relative to the counter
            PositionCustomer();
            
            // Create interaction trigger
            CreateInteractionTrigger();
        }
        
        private void PositionCustomer()
        {
            if (kitchenCounter != null)
            {
                // Get counter position and add offset
                Vector3 targetPosition = kitchenCounter.position + counterOffset;
                transform.position = targetPosition;
                
                // Make customer face the counter
                transform.LookAt(new Vector3(kitchenCounter.position.x, transform.position.y, kitchenCounter.position.z));
            }
        }
        
        private void CreateInteractionTrigger()
        {
            // Create a trigger collider for food interaction
            SphereCollider interactionCollider = gameObject.AddComponent<SphereCollider>();
            interactionCollider.radius = interactionRange;
            interactionCollider.isTrigger = true;
            
            // If there's already a collider for physics, make sure it's not a trigger
            Collider[] existingColliders = GetComponents<Collider>();
            foreach (Collider col in existingColliders)
            {
                if (col != interactionCollider)
                {
                    col.isTrigger = false;
                }
            }
        }
        
        // Optionally, visualize the interaction range in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            if (kitchenCounter != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, kitchenCounter.position);
            }
        }
    }
}
