using UnityEngine;
using TMPro;
using System.Collections;

namespace SojaExiles
{
    public class CustomerFoodRequest : MonoBehaviour
    {
        private TextMeshPro requestText;
        private FoodType desiredFood;
        private bool hasOrder = false;

        void Awake()
        {
            InitializeTextComponent();
            GenerateRandomFoodRequest(); // Generate initial food request
            SetTextActive(true); // Make sure text is visible immediately
        }

        private void InitializeTextComponent()
        {
            // First try to find existing TextMeshPro component
            requestText = GetComponentInChildren<TextMeshPro>();
            
            if (requestText == null)
            {
                // Create a new GameObject for the text
                GameObject textObj = new GameObject("RequestText");
                textObj.transform.SetParent(transform);
                
                // Position it above the customer's head
                textObj.transform.localPosition = Vector3.up * 2.5f;
                textObj.transform.localRotation = Quaternion.identity;
                
                // Add TextMeshPro component
                requestText = textObj.AddComponent<TextMeshPro>();
                
                // Configure the text component
                requestText.alignment = TextAlignmentOptions.Center;
                requestText.fontSize = 4f;
                requestText.color = Color.black;
                
                // Set font settings
                requestText.enableWordWrapping = false;
                requestText.overflowMode = TextOverflowModes.Overflow;
                requestText.margin = new Vector4(2, 2, 2, 2);
                requestText.horizontalAlignment = HorizontalAlignmentOptions.Center;
                requestText.verticalAlignment = VerticalAlignmentOptions.Middle;
                
                Debug.Log($"[{gameObject.name}] Created new TextMeshPro component");
            }
            
            // Set initial text
            if (requestText != null)
            {
                requestText.text = "Deciding what to order...";
                SetTextActive(true);
            }
        }

        private void SetTextActive(bool active)
        {
            if (requestText != null && requestText.gameObject != null)
            {
                requestText.gameObject.SetActive(active);
                Debug.Log($"[{gameObject.name}] Set text visibility to: {active}");
            }
        }

        void Start()
        {
            // Verify text component
            if (requestText == null)
            {
                Debug.LogError($"[{gameObject.name}] TextMeshPro component is missing!");
                InitializeTextComponent();
            }
            
            // Make sure order is displayed
            UpdateRequestDisplay();
            SetTextActive(true);
        }

        public void SetAtCounter(bool atCounter)
        {
            if (requestText == null)
            {
                Debug.LogError($"[{gameObject.name}] TextMeshPro component is missing!");
                return;
            }

            UpdateRequestDisplay();
            SetTextActive(atCounter && hasOrder);
            if (atCounter && hasOrder)
            {
                UpdateRequestDisplay();
            }
        }

        public void GenerateRandomFoodRequest()
        {
            desiredFood = Random.value < 0.5f ? FoodType.pizza : FoodType.hotdog;
            hasOrder = true;
            Debug.Log($"[{gameObject.name}] Generated new order: {desiredFood}");
            UpdateRequestDisplay();
            SetTextActive(true); // Make sure text is visible after generating order
        }

        private void UpdateRequestDisplay()
        {
            if (requestText == null)
            {
                Debug.LogError($"[{gameObject.name}] TextMeshPro component is missing!");
                return;
            }

            // Set the request text
            requestText.text = $"I want {desiredFood}!";
            requestText.color = Color.black;
            requestText.fontSize = 4f;
            
            // Make text face the camera
            if (Camera.main != null)
            {
                requestText.transform.rotation = Camera.main.transform.rotation;
            }
            
            Debug.Log($"[{gameObject.name}] Updated display text: {requestText.text}");
        }

        public FoodType GetDesiredFood()
        {
            return desiredFood;
        }

        public void ShowResponse(bool isCorrectOrder)
        {
            if (requestText == null)
            {
                InitializeTextComponent();
                if (requestText == null)
                {
                    Debug.LogError($"[{gameObject.name}] Failed to initialize TextMeshPro component!");
                    return;
                }
            }

            // Ensure text is in the correct position and visible
            requestText.transform.localPosition = Vector3.up * 2.5f;
            requestText.gameObject.SetActive(true);
            
            if (isCorrectOrder)
            {
                // Random positive responses
                string[] positiveResponses = {
                    "Thank you!",
                    "Perfect!",
                    "Just what I wanted!",
                    "Delicious!"
                };
                requestText.text = positiveResponses[Random.Range(0, positiveResponses.Length)];
                requestText.color = Color.green;
            }
            else
            {
                // Random negative responses
                string[] negativeResponses = {
                    "Screw you!",
                    "This is wrong!",
                    "Not what I ordered!",
                    "Are you kidding me?"
                };
                requestText.text = negativeResponses[Random.Range(0, negativeResponses.Length)];
                requestText.color = Color.red;
            }
            
            // Make text more visible
            requestText.fontSize = 5f;
            requestText.enableWordWrapping = false;
            requestText.alignment = TextAlignmentOptions.Center;

            // Make text face the camera
            if (Camera.main != null)
            {
                requestText.transform.rotation = Camera.main.transform.rotation;
            }
            
            Debug.Log($"[{gameObject.name}] Showing response: {requestText.text}");
            
            // Keep the text visible a bit longer
            StartCoroutine(HideTextAfterDelay(3f));
        }

        private IEnumerator HideTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
            }
            ClearRequest();
        }

        public void ClearRequest()
        {
            hasOrder = false;
            SetTextActive(false);
            Debug.Log($"[{gameObject.name}] Request cleared and text hidden");
        }

        void Update()
        {
            // Keep text facing the camera
            if (requestText != null && requestText.gameObject.activeInHierarchy && Camera.main != null)
            {
                requestText.transform.rotation = Camera.main.transform.rotation;
                
                // If text is not visible but should be, make it visible
                if (!requestText.gameObject.activeSelf && hasOrder)
                {
                    SetTextActive(true);
                    UpdateRequestDisplay();
                }
            }
        }

        void OnEnable()
        {
            // Make sure text is visible when object is enabled
            if (hasOrder)
            {
                SetTextActive(true);
                UpdateRequestDisplay();
            }
        }
    }
}
