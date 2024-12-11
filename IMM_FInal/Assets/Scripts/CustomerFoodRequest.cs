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
        private Coroutine hideTextCoroutine;
        private bool hasReceivedFood = false;
        private bool isAtCounter = false;

        void Awake()
        {
            InitializeTextComponent();
            GenerateRandomFoodRequest();
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
            requestText.gameObject.SetActive(false);
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
            if (!hasReceivedFood)  // Only generate if haven't received food
            {
                desiredFood = Random.value < 0.5f ? FoodType.Pizza : FoodType.Hotdog;
                hasOrder = true;
                ShowRequest(); // Show the request immediately
            }
        }

        public void ShowResponse(bool isCorrectOrder)
        {
            hasReceivedFood = true;  // Mark that we've received food
            
            if (requestText == null)
            {
                Debug.LogWarning($"[{gameObject.name}] Request text was null, initializing");
                InitializeTextComponent();
            }

            // Replace the order text with the response
            requestText.gameObject.SetActive(true);

            if (isCorrectOrder)
            {
                requestText.text = "Thank you!";
                requestText.color = new Color(0, 0.8f, 0); // Bright green
                Debug.Log($"[{gameObject.name}] Showing thank you response");
            }
            else
            {
                requestText.text = "This is not what I ordered!";
                requestText.color = new Color(0.8f, 0, 0); // Bright red
                Debug.Log($"[{gameObject.name}] Showing wrong order response");
            }

            // Start coroutine to hide text after delay
            if (hideTextCoroutine != null)
            {
                StopCoroutine(hideTextCoroutine);
            }
            hideTextCoroutine = StartCoroutine(HideTextAfterDelay(2f));
        }

        public void ShowRequest()
        {
            if (requestText != null && hasOrder && !hasReceivedFood)  // Only show if haven't received food
            {
                requestText.gameObject.SetActive(true);
                requestText.text = $"I want {desiredFood}!";
                requestText.color = Color.black;
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

        private IEnumerator HideTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideRequest();
        }
    }
}
