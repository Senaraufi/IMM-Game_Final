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
        private bool isAtCounter = false;

        void Start()
        {
            InitializeTextComponent();
            GenerateRandomFoodRequest();
            
            // Make sure text starts hidden
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
            }
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
                textObj.transform.localPosition = new Vector3(0, 2.5f, 0);
                textObj.transform.localRotation = Quaternion.identity;
                
                // Add TextMeshPro component
                requestText = textObj.AddComponent<TextMeshPro>();
                
                // Configure the text component
                requestText.alignment = TextAlignmentOptions.Center;
                requestText.fontSize = 3f;
                requestText.color = Color.black;
                requestText.enableWordWrapping = true;
                requestText.overflowMode = TextOverflowModes.Overflow;
                requestText.margin = new Vector4(2, 2, 2, 2);
            }

            // Ensure text is properly configured
            requestText.enableWordWrapping = true;
            requestText.alignment = TextAlignmentOptions.Center;
            requestText.fontSize = 3f;
        }

        public void GenerateRandomFoodRequest()
        {
            // Get a random food type
            FoodType[] foodTypes = (FoodType[])System.Enum.GetValues(typeof(FoodType));
            desiredFood = foodTypes[Random.Range(0, foodTypes.Length)];
            
            if (requestText != null)
            {
                requestText.text = $"I want {desiredFood}!";
                requestText.gameObject.SetActive(isAtCounter);
            }
            hasOrder = true;
        }

        public void SetAtCounter(bool atCounter)
        {
            isAtCounter = atCounter;
            if (requestText != null)
            {
                requestText.gameObject.SetActive(atCounter && hasOrder);
                if (atCounter && hasOrder)
                {
                    requestText.text = $"I want {desiredFood}!";
                    requestText.color = Color.black;
                }
            }
            Debug.Log($"[{gameObject.name}] Set at counter: {atCounter}, Text visible: {requestText != null && requestText.gameObject.activeSelf}");
        }

        public void ShowRequest()
        {
            if (requestText != null && hasOrder)
            {
                requestText.gameObject.SetActive(true);
                requestText.text = $"I want {desiredFood}!";
                requestText.color = Color.black;
            }
            else if (requestText == null)
            {
                InitializeTextComponent();
                GenerateRandomFoodRequest();
            }
        }

        public void ShowResponse(bool isCorrectOrder)
        {
            if (requestText == null)
            {
                InitializeTextComponent();
            }

            if (requestText != null)
            {
                // Make sure text object is active
                requestText.gameObject.SetActive(true);
                
                if (isCorrectOrder)
                {
                    string[] positiveResponses = {
                        "Thank you! This is perfect!",
                        "Yummy! Just what I wanted!",
                        "Amazing! You got it right!",
                        "Delicious! Great service!"
                    };
                    requestText.text = positiveResponses[Random.Range(0, positiveResponses.Length)];
                    requestText.color = new Color(0, 0.8f, 0); // Bright green
                }
                else
                {
                    string[] negativeResponses = {
                        "This is not what I ordered!",
                        "Wrong food! I wanted something else!",
                        "No, no, no! This isn't right!",
                        "That's completely wrong!"
                    };
                    requestText.text = negativeResponses[Random.Range(0, negativeResponses.Length)];
                    requestText.color = new Color(0.8f, 0, 0); // Bright red
                }
                
                // Make text more visible
                requestText.fontSize = 3f;
                requestText.outlineWidth = 0.2f;
                requestText.outlineColor = Color.white;
                
                // Start coroutine to hide text after delay
                StartCoroutine(HideTextAfterDelay(3f));
            }
        }

        private IEnumerator HideTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
            }
        }

        public void HideRequest()
        {
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
                hasOrder = false;
            }
        }

        void Update()
        {
            // Keep text facing the camera
            if (requestText != null && requestText.gameObject.activeInHierarchy && Camera.main != null)
            {
                requestText.transform.rotation = Camera.main.transform.rotation;
            }
        }

        public FoodType GetDesiredFood()
        {
            return desiredFood;
        }

        public bool HasOrder()
        {
            return hasOrder;
        }
    }
}
