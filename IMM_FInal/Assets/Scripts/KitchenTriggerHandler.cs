using UnityEngine;

namespace SojaExiles
{
    public class KitchenTriggerHandler : MonoBehaviour
    {
        [SerializeField]
        private TextHide textHideScript; // Reference to the TextHide script

        [SerializeField]
        private string triggerMessage = "Can I have a pizza?";

        void Start()
        {
            if(textHideScript == null)
            {
                Debug.LogError("TextHide script is not assigned in KitchenTriggerHandler.");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                textHideScript.ShowText(triggerMessage);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                textHideScript.HideText();
            }
        }
    }
} 