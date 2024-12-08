using UnityEngine;

namespace SojaExiles
{
    public class FoodScript : MonoBehaviour
    {
        [SerializeField]
        private FoodType foodType = FoodType.pizza;
        
        void Start()
        {
            // Log the food type when the object is created
            Debug.Log($"[{gameObject.name}] Food type initialized as: {foodType}");
        }
        
        public FoodType FoodType 
        { 
            get 
            {
                Debug.Log($"[{gameObject.name}] Getting food type: {foodType}");
                return foodType;
            }
        }
    }
}
