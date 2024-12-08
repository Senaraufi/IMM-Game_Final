using UnityEngine;

namespace SojaExiles
{
    public class FoodScript : MonoBehaviour
    {
        [SerializeField]
        private FoodType foodType = FoodType.pizza;
        
        public FoodType FoodType => foodType;
    }
}
