using UnityEngine;
using TMPro;
using System.Collections;

namespace SojaExiles
{
    public class CustomerFoodRequest : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro requestText;
        
        private FoodType desiredFood;
        private bool hasOrder = false;

        void Start()
        {
            GenerateRandomFoodRequest();
            
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
                Debug.Log($"[{gameObject.name}] CustomerFoodRequest initialized with text component");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Request text component is not assigned!");
            }
        }

        public void SetAtCounter(bool atCounter)
        {
            if (requestText != null)
            {
                requestText.gameObject.SetActive(atCounter && hasOrder);
                Debug.Log($"[{gameObject.name}] Text visibility set to: {atCounter && hasOrder}");
            }
        }

        public void GenerateRandomFoodRequest()
        {
            desiredFood = Random.value < 0.5f ? FoodType.pizza : FoodType.hotdog;
            hasOrder = true;
            Debug.Log($"[{gameObject.name}] Generated new order:");
            Debug.Log($"  - Food Type: {desiredFood}");
            Debug.Log($"  - Food Type Value: {(int)desiredFood}");
            Debug.Log($"  - Food Type String: {desiredFood.ToString()}");
            UpdateRequestDisplay();
        }

        private void UpdateRequestDisplay()
        {
            if (requestText != null && hasOrder)
            {
                requestText.text = $"I want {desiredFood}";
                requestText.color = Color.black;
                Debug.Log($"[{gameObject.name}] Updated display text: {requestText.text}");
            }
        }

        public FoodType GetDesiredFood()
        {
            Debug.Log($"[{gameObject.name}] GetDesiredFood called:");
            Debug.Log($"  - Returning Food Type: {desiredFood}");
            Debug.Log($"  - Food Type Value: {(int)desiredFood}");
            Debug.Log($"  - Food Type String: {desiredFood.ToString()}");
            return desiredFood;
        }

        public void ShowResponse(bool correctOrder)
        {
            Debug.Log($"[{gameObject.name}] ShowResponse called - correctOrder: {correctOrder}");
            
            if (requestText != null)
            {
                requestText.gameObject.SetActive(true);
                
                if (correctOrder)
                {
                    requestText.text = "Thank you!";
                    requestText.color = Color.green;
                    Debug.Log($"[{gameObject.name}] Showing thank you message");
                }
                else
                {
                    requestText.text = "Screw you!";
                    requestText.color = Color.red;
                    Debug.Log($"[{gameObject.name}] Showing angry message");
                }
                
                StartCoroutine(HideTextAfterDelay(2f));
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Cannot show response - text component is missing!");
            }
        }

        private IEnumerator HideTextAfterDelay(float delay)
        {
            Debug.Log($"[{gameObject.name}] Starting delay to hide text");
            yield return new WaitForSeconds(delay);
            Debug.Log($"[{gameObject.name}] Hiding text after delay");
            ClearRequest();
        }

        public void ClearRequest()
        {
            hasOrder = false;
            if (requestText != null)
            {
                requestText.gameObject.SetActive(false);
                Debug.Log($"[{gameObject.name}] Request cleared and text hidden");
            }
        }
    }
}
