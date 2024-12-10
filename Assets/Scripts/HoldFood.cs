using UnityEngine;

namespace SojaExiles
{
    public class HoldFood : MonoBehaviour
    {
        private GameObject heldObject = null;
        private Camera mainCamera;
        private Vector3 holdOffset = new Vector3(0, 0, 1); // Adjust as needed for holding position

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
                    Debug.Log($"{heldObject.name} has been picked up.");
                }
                else
                {
                    Debug.Log("No food object found to pick up.");
                }

            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }

        public LayerMask ignoreMe;

        private void TryPlaceObject()
        {
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 5f, ~ignoreMe)) // Adjust max distance as needed
            {   
                if (hit.collider.CompareTag("Customer")) {
                    var haveType = heldObject.GetComponent<FoodScript>().FoodType;

                    var result = hit.collider.gameObject.GetComponent<AcceptFood>().AcceptFoodItem(haveType, heldObject);
                    if (result) {
                        return;
                    }
                }
                heldObject.transform.SetParent(null);
                heldObject.transform.position = hit.point;
                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
                Debug.Log($"{heldObject.name} has been placed.");
                heldObject = null;
            }
            else
            {
                Debug.Log("Cannot place the object here. No valid surface hit.");
            }
        }
    }
}