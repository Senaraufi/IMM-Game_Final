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
        private float showDuration = 3f;
        private bool isShowing = false;
        private Coroutine fadeCoroutine;
        private bool isAtCounter = false;
        private Coroutine hideTextCoroutine;

        void Start()
        {
            InitializeTextComponent();
        }

        private void InitializeTextComponent()
        {
            requestText = GetComponentInChildren<TextMeshPro>();
            if (requestText == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Creating new TextMeshPro");
                GameObject textObj = new GameObject("RequestText");
                textObj.transform.SetParent(transform);
                requestText = textObj.AddComponent<TextMeshPro>();
                
                // Position the text above the customer
                textObj.transform.localPosition = new Vector3(0, 2f, 0);
                textObj.transform.localRotation = Quaternion.identity;
            }

            // Set default text properties
            requestText.alignment = TextAlignmentOptions.Center;
            requestText.fontSize = 3f;
            requestText.outlineWidth = 0.2f;
            requestText.outlineColor = Color.black;
            requestText.gameObject.SetActive(true);
        }

        void Update()
        {
            // Keep text facing the camera
            if (requestText != null && requestText.gameObject.activeInHierarchy && Camera.main != null)
            {
                requestText.transform.LookAt(Camera.main.transform);
                requestText.transform.Rotate(0, 180, 0); // Make text face the camera
            }
        }

        public void GenerateRandomFoodRequest()
        {
            // Randomly select a food type
            desiredFood = Random.value < 0.5f ? FoodType.Pizza : FoodType.Hotdog;
            hasOrder = true;
        }

        public void ShowResponse(bool isCorrectOrder)
        {
            Debug.Log($"[{gameObject.name}] ShowResponse called with isCorrectOrder: {isCorrectOrder}");
            
            if (requestText == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Request text was null, initializing");
                InitializeTextComponent();
            }

            // Make sure text object is active and facing camera
            requestText.gameObject.SetActive(true);
            requestText.transform.LookAt(Camera.main.transform);
            requestText.transform.Rotate(0, 180, 0); // Make text face the camera
            
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
                Debug.Log($"[{gameObject.name}] Set positive response: {requestText.text}");
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
                Debug.Log($"[{gameObject.name}] Set negative response: {requestText.text}");
            }
            
            // Make text more visible
            requestText.fontSize = 3f;
            requestText.outlineWidth = 0.2f;
            requestText.outlineColor = Color.black;
            
            // Cancel any existing hide coroutine
            if (hideTextCoroutine != null)
            {
                StopCoroutine(hideTextCoroutine);
            }
            
            // Start new hide coroutine
            hideTextCoroutine = StartCoroutine(HideTextAfterDelay(3f));
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

        public FoodType GetDesiredFood()
        {
            return desiredFood;
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
    }
}
