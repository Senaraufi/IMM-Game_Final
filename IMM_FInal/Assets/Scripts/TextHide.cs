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

    // Method to display text
    public void DisplayText(string message)
    {
        if(text != null)
        {
            text.text = message;
            text.enabled = true;
        }
    }

    // Method to hide text
    public void HideText()
    {
        if(text != null)
        {
            text.text = "";
            text.enabled = false;
        }
    }

    // Method to toggle text
    public void ToggleText()
    {
        if(text != null)
        {
            text.enabled = !text.enabled;
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
