using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI titleText;
    public GameObject difficultyButtons;

    private void Start()
    {
        // Set up the title text
        if (titleText != null)
        {
            titleText.text = "Cooking Game";
            titleText.fontSize = 72;
            titleText.color = Color.white;
        }

        // Ensure difficulty buttons are visible
        if (difficultyButtons != null)
        {
            difficultyButtons.SetActive(true);
        }
    }

    // These methods will be called by the UI buttons
    public void OnEasySelected()
    {
        GameManager.Instance.SetDifficulty(0);
    }

    public void OnMediumSelected()
    {
        GameManager.Instance.SetDifficulty(1);
    }

    public void OnHardSelected()
    {
        GameManager.Instance.SetDifficulty(2);
    }
}
