using UnityEngine;
using TMPro;

namespace SojaExiles
{
    public class TextHide : MonoBehaviour
    {
        public static TextHide Instance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI messageText;
        [SerializeField]
        private TextMeshProUGUI pressEText;

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

            if (messageText != null) messageText.enabled = false;
            if (pressEText != null) pressEText.enabled = false;
        }

        public void ShowText(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.enabled = true;
            }
        }

        public void ShowPressE()
        {
            if (pressEText != null)
            {
                pressEText.text = "Press E to pick up food";
                pressEText.enabled = true;
            }
        }

        public void HideText()
        {
            if (messageText != null)
            {
                messageText.enabled = false;
            }
        }

        public void HidePressE()
        {
            if (pressEText != null)
            {
                pressEText.enabled = false;
            }
        }
    }
}
