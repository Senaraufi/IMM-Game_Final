using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SojaExiles
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button startButton;
        public TextMeshProUGUI titleText;
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
}
