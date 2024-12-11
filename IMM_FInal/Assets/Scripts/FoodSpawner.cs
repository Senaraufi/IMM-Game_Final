using UnityEngine;

namespace SojaExiles
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject pizzaPrefab;    // Pizza prefab
        [SerializeField] private GameObject hotdogPrefab;   // Hotdog prefab
        [SerializeField] private Transform pizzaSpawnPoint; // Where pizza spawns
        [SerializeField] private Transform hotdogSpawnPoint;// Where hotdog spawns
        [SerializeField] private float spawnDelay = 0.5f;  // Short delay before spawning new food

        private static FoodSpawner instance;
        private FoodType foodToSpawn;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public static void RespawnFood(FoodType type)
        {
            if (instance != null)
            {
                instance.foodToSpawn = type;
                instance.Invoke("SpawnFood", instance.spawnDelay);
            }
        }

        private void SpawnFood()
        {
            if (foodToSpawn == FoodType.pizza && pizzaPrefab != null && pizzaSpawnPoint != null)
            {
                GameObject newFood = Instantiate(pizzaPrefab, pizzaSpawnPoint.position, pizzaSpawnPoint.rotation);
            }
            else if (foodToSpawn == FoodType.hotdog && hotdogPrefab != null && hotdogSpawnPoint != null)
            {
                GameObject newFood = Instantiate(hotdogPrefab, hotdogSpawnPoint.position, hotdogSpawnPoint.rotation);
            }
        }
    }
}
