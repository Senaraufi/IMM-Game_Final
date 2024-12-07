using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AcceptFood : MonoBehaviour
{
    [SerializeField]
    private TextHide textHide;

    [SerializeField]
    private FoodType acceptedFoodType = FoodType.pizza;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            FoodScript foodScript = other.gameObject.GetComponent<FoodScript>();
            if (foodScript == null)
            {
                Debug.LogError("FoodScript component not found on the Food GameObject.");
                return;
            }

            FoodType incomingFoodType = foodScript.FoodType;

            if (incomingFoodType != acceptedFoodType)
            {
                textHide.ShowText("FUCK OFF");
            }
            else
            {
                textHide.ShowText("Food accepted");
                Debug.Log("Food accepted");
            }
        }
    }

    public bool AcceptFoodItem(FoodType type, GameObject heldFood)
    {
        if (type != acceptedFoodType)
        {
            textHide.ShowText("FUCK OFF");
            return false;
        }
        else
        {
            textHide.ShowText("Food accepted");
            Destroy(heldFood);
            return true;
        }
    }
}
