using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace SojaExiles
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button startButton;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI linkText;  // Reference to the text with the link
        public GameManager gameManager;

        void Start()
        {
            Debug.Log("MainMenu Start called");
            
            // Set up button listener
            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
                Debug.Log("Start button listener added");
            }
            else
            {
                Debug.LogError("Start button not assigned!");
            }

            // Set up title text
            if (titleText != null)
            {
                titleText.text = "Cooking Game";
                titleText.fontSize = 72;
                titleText.color = Color.white;
                Debug.Log("Title text set up");
            }

            // Set up link text
            if (linkText != null)
            {
                // Make the text support rich text and links
                linkText.richText = true;
                linkText.enableWordWrapping = true;

                // Add the link using TMPro's link tag
                linkText.text = "Visit our website: <link=\"https://example.com\"><u>Click here</u></link>";
                
                // Add click event listener
                linkText.gameObject.AddComponent<ClickableLink>();
            }
        }

        void OnStartClicked()
        {
            Debug.Log("Start button clicked!");
            if (gameManager != null)
            {
                gameManager.StartGame();
            }
            else
            {
                Debug.LogError("GameManager not assigned!");
            }
        }

        void OnDestroy()
        {
            if (startButton != null)
            {
                startButton.onClick.RemoveListener(OnStartClicked);
            }
        }
    }

    // Component to handle link clicks
    public class ClickableLink : MonoBehaviour, IPointerClickHandler
    {
        private TextMeshProUGUI textComponent;

        void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, null);
            if (linkIndex != -1)
            {
                // Get link info
                TMP_LinkInfo linkInfo = textComponent.textInfo.linkInfo[linkIndex];
                
                // Open the URL
                string url = linkInfo.GetLinkID();
                Application.OpenURL(url);
                
                Debug.Log($"Opening URL: {url}");
            }
        }
    }
}
