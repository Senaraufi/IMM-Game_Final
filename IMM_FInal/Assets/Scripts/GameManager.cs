using UnityEngine;
using UnityEngine.UI;

namespace SojaExiles
{
    public class GameManager : MonoBehaviour
    {
        public GameObject mainMenuPanel;
        public Button startButton;

        void Start()
        {
            // Make sure the menu is visible at start
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }

            // Set up button click listener if button exists
            if (startButton != null)
            {
                startButton.onClick.AddListener(StartGame);
            }
        }

        public void StartGame()
        {
            Debug.Log("Button clicked - Starting game");
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
                Debug.Log("Main menu hidden");
            }
        }
    }
}
