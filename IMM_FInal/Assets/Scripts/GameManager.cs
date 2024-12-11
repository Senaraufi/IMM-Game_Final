using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public Difficulty currentDifficulty { get; private set; }
    public bool gameStarted { get; private set; } = false;

    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject gameUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure game starts in menu state
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        Time.timeScale = 0; // Pause the game
    }

    public void SetDifficulty(int difficultyLevel)
    {
        currentDifficulty = (Difficulty)difficultyLevel;
        StartGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        Time.timeScale = 1; // Resume the game

        // Apply difficulty settings
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                SetEasyDifficulty();
                break;
            case Difficulty.Medium:
                SetMediumDifficulty();
                break;
            case Difficulty.Hard:
                SetHardDifficulty();
                break;
        }
    }

    private void SetEasyDifficulty()
    {
        // Set easy mode parameters
        // Example: slower customer spawn rate, more time to prepare food
        var customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.spawnInterval = 20f;
        }
    }

    private void SetMediumDifficulty()
    {
        // Set medium mode parameters
        var customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.spawnInterval = 15f;
        }
    }

    private void SetHardDifficulty()
    {
        // Set hard mode parameters
        // Example: faster customer spawn rate, less time to prepare food
        var customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (customerSpawner != null)
        {
            customerSpawner.spawnInterval = 10f;
        }
    }
}
