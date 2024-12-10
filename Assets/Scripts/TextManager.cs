using UnityEngine;
using TMPro;

namespace SojaExiles
{
    public class TextManager : MonoBehaviour
    {
        public static TextManager Instance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI customerMessageText;
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

            HideAllText();
        }

        public void ShowCustomerMessage(string message)
        {
            if (customerMessageText != null)
            {
                customerMessageText.text = message;
                customerMessageText.enabled = true;
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

        public void HideCustomerMessage()
        {
            if (customerMessageText != null)
            {
                customerMessageText.enabled = false;
            }
        }

        public void HidePressE()
        {
            if (pressEText != null)
            {
                pressEText.enabled = false;
            }
        }

        public void HideAllText()
        {
            HideCustomerMessage();
            HidePressE();
        }
    }
}
