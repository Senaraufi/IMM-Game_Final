using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHide : MonoBehaviour
{

    [SerializeField]
    public TextMeshProUGUI text;
    public TextMeshProUGUI pressE;

    // Start is called before the first frame update
    void Start()
    { 
        HideAllText();
    }

    // Method to show text
    public void ShowText(string message)
    {
        if(text != null)
        {
            text.text = message;
            text.enabled = true;
            Debug.Log($"Text displayed: {message}");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned.");
        }
    }

    // Method to hide text
    public void HideText()
    {
        if(text != null)
        {
            text.text = "";
            text.enabled = false;
            Debug.Log("Text has been hidden.");
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component is not assigned.");
        }
    }

    private void HideAllText()
    {
        if(text != null)
        {
            text.text = "";
            text.enabled = false;
        }

        if(pressE != null)
        {
            pressE.text = "";
            pressE.enabled = false;
        }
    }
}
