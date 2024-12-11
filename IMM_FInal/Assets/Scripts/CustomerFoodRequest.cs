using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

namespace SojaExiles
{
    public class CustomerFoodRequest : MonoBehaviour
    {
        public GameObject requestBubble;
        public TMP_Text requestText;
        public Image foodIcon;
        public Sprite[] foodIcons;
        public float showDuration = 3f;
        public float fadeSpeed = 2f;
        private FoodType desiredFood;
        private CanvasGroup canvasGroup;
        private bool isShowing = false;
        private Coroutine fadeCoroutine;
        private bool isAtCounter = false;

        void Start()
        {
            if (requestBubble != null)
            {
                canvasGroup = requestBubble.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = requestBubble.AddComponent<CanvasGroup>();
                }
                requestBubble.SetActive(true);
                canvasGroup.alpha = 0;
            }

            GenerateRandomFoodRequest();
        }

        void Update()
        {
            // Make the request bubble always face the camera without flipping
            if (requestBubble != null && requestBubble.activeSelf)
            {
                requestBubble.transform.forward = Camera.main.transform.forward;
            }
        }

        public void GenerateRandomFoodRequest()
        {
            // Get a random food type
            FoodType[] foodTypes = (FoodType[])System.Enum.GetValues(typeof(FoodType));
            desiredFood = foodTypes[Random.Range(0, foodTypes.Length)];

            // Update UI
            if (requestText != null)
            {
                requestText.text = $"I want a {desiredFood}!";
            }

            if (foodIcon != null && foodIcons != null && foodIcons.Length > 0)
            {
                int iconIndex = (int)desiredFood;
                if (iconIndex < foodIcons.Length)
                {
                    foodIcon.sprite = foodIcons[iconIndex];
                }
            }
        }

        public void ShowRequest()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeIn());
        }

        public void HideRequest()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeOut());
        }

        public void SetAtCounter(bool atCounter)
        {
            isAtCounter = atCounter;
            if (atCounter)
            {
                ShowRequest();
            }
            else
            {
                HideRequest();
            }
        }

        public bool IsAtCounter()
        {
            return isAtCounter;
        }

        private IEnumerator FadeIn()
        {
            if (canvasGroup != null)
            {
                while (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                    yield return null;
                }
                isShowing = true;
            }
        }

        private IEnumerator FadeOut()
        {
            if (canvasGroup != null)
            {
                while (canvasGroup.alpha > 0)
                {
                    canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                    yield return null;
                }
                isShowing = false;
            }
        }

        public void ShowResponse(bool isCorrect)
        {
            if (requestText != null)
            {
                requestText.text = isCorrect ? "Thank you!" : "That's not what I wanted...";
                ShowRequest();
                StartCoroutine(HideAfterDelay(showDuration));
            }
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideRequest();
        }

        public FoodType GetDesiredFood()
        {
            return desiredFood;
        }

        public bool IsShowing()
        {
            return isShowing;
        }
    }
}
