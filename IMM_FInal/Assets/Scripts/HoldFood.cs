using UnityEngine;

namespace SojaExiles
{
    public class HoldFood : MonoBehaviour
    {
        private GameObject heldObject = null;
        private Camera mainCamera;
        private Vector3 holdOffset = new Vector3(0, 0, 1); // Adjust as needed for holding position
        public PointsManager pointsManager;

        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (heldObject == null)
                {
                    TryPickUpObject();
                }
                else
                {
                    TryPlaceObject();
                }
            }

            if (heldObject != null)
            {
                // Optional: Smoothly follow the hold position
                Vector3 desiredPosition = mainCamera.transform.position + mainCamera.transform.TransformDirection(holdOffset);
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, desiredPosition, Time.deltaTime * 10f);
            }
        }

        private void TryPickUpObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f)) // Adjust max distance as needed
            {
                if (hit.collider.CompareTag("Food"))
                {
                    heldObject = hit.collider.gameObject;
                    heldObject.transform.SetParent(this.transform);
                    heldObject.transform.localPosition = holdOffset;
                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                    }
                }
            }
        }

        public LayerMask ignoreMe;

        private void TryPlaceObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f, ~ignoreMe))
            {
                if (hit.collider.CompareTag("Customer"))
                {
                    if (heldObject == null)
                    {
                        Debug.LogWarning("Trying to deliver but no food is held!");
                        return;
                    }

                    var foodScript = heldObject.GetComponent<FoodScript>();
                    if (foodScript == null)
                    {
                        Debug.LogError("Held object has no FoodScript!");
                        return;
                    }

                    var customer = hit.collider.gameObject.GetComponent<AcceptFood>();
                    if (customer == null)
                    {
                        Debug.LogError("Customer has no AcceptFood component!");
                        return;
                    }

                    Debug.Log($"Attempting delivery - Food: {foodScript.FoodType}");
                    bool isCorrect = customer.AcceptFoodItem(foodScript.FoodType, heldObject);
                    
                    if (pointsManager != null)
                    {
                        if (isCorrect)
                        {
                            Debug.Log("Delivery successful, adding points");
                            pointsManager.AddPoints(10);
                            // Let AcceptFood handle the food destruction
                            heldObject = null;
                        }
                        else
                        {
                            Debug.Log("Delivery failed, subtracting points");
                            pointsManager.AddPoints(-5);
                            // Let AcceptFood handle the food destruction
                            heldObject = null;
                        }
                    }
                    else
                    {
                        Debug.LogError("PointsManager is missing!");
                    }
                }
                else
                {
                    // Generic placement if not a customer
                    heldObject.transform.SetParent(null);
                    heldObject.transform.position = hit.point;
                    Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                    }
                    heldObject = null;
                }
            }
        }
    }
}