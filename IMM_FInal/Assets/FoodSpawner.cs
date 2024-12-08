using UnityEngine;

namespace SojaExiles
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject foodPrefab;    // Assign your food prefab
        [SerializeField] private Transform spawnPoint;     // Where food will spawn
        [SerializeField] private float spawnDelay = 0.5f;  // Short delay before spawning new food

        private static FoodSpawner instance;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            // Spawn initial food
            SpawnFood();
        }

        public static void RespawnFood()
        {
            if (instance != null)
            {
                instance.Invoke("SpawnFood", instance.spawnDelay);
            }
        }

        private void SpawnFood()
        {
            if (foodPrefab != null && spawnPoint != null)
            {
                GameObject newFood = Instantiate(foodPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"[{gameObject.name}] New food spawned at counter");
            }
            else
            {
                Debug.LogError($"[{gameObject.name}] Food prefab or spawn point not assigned!");
            }
        }
    }
}
