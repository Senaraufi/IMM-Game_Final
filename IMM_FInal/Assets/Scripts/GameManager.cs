using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SojaExiles
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuPanel;  // Serialized for Unity Inspector

        void Awake()
        {
            // Try to find the panel if it's not assigned
            if (mainMenuPanel == null)
            {
                mainMenuPanel = transform.Find("MainMenuPanel")?.gameObject;
                if (mainMenuPanel == null)
                {
                    mainMenuPanel = GameObject.Find("MainMenuPanel");
                }
            }
        }

        void Start()
        {
            if (mainMenuPanel == null)
            {
                Debug.LogError("MainMenuPanel is not assigned! Please assign it in the Inspector.");
                return;
            }
            
            mainMenuPanel.SetActive(true);
            Debug.Log("MainMenuPanel is ready and visible");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Q key pressed - Starting game");
                StartGame();
            }
        }

        public void StartGame()  // Made public so MainMenu.cs can access it
        {
            if (mainMenuPanel == null)
            {
                Debug.LogError("MainMenuPanel is not assigned!");
                return;
            }

            Debug.Log("Hiding main menu panel");
            mainMenuPanel.SetActive(false);
        }
    }
}
