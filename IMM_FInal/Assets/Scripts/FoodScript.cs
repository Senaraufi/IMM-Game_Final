using UnityEngine;

namespace SojaExiles
{
    public class FoodScript : MonoBehaviour
    {
        [SerializeField]
        private FoodType foodType = FoodType.Pizza;
        
        void Start()
        {
            Debug.Log($"[{gameObject.name}] Food type initialized as: {foodType}");
        }

        void OnDestroy()
        {
            // Only request respawn if this is a proper destroy (not scene unload)
            if (gameObject.scene.isLoaded)
            {
                Debug.Log($"[{gameObject.name}] Food destroyed, requesting respawn of type: {foodType}");
                FoodSpawner.RespawnFood(foodType);
            }
        }
        
        public void SetFoodType(FoodType type)
        {
            foodType = type;
            Debug.Log($"[{gameObject.name}] Food type set to: {foodType}");
        }
        
        public FoodType FoodType 
        { 
            get 
            {
                return foodType;
            }
        }
    }
}
