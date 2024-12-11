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
        public static FoodSpawner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<FoodSpawner>();
                    if (instance == null)
                    {
                        Debug.LogError("No FoodSpawner found in the scene!");
                    }
                }
                return instance;
            }
        }

        void Start()
        {
            // Spawn initial food items
            SpawnFood(FoodType.Pizza);
            SpawnFood(FoodType.Hotdog);
            Debug.Log("Initial food items spawned");
        }

        public static void RespawnFood(FoodType type)
        {
            if (Instance != null)
            {
                Debug.Log($"Respawning food of type: {type}");
                Instance.Invoke("SpawnFood", Instance.spawnDelay, type);
            }
        }

        private void SpawnFood(FoodType type)
        {
            GameObject prefab = null;
            Transform spawnPoint = null;
            
            switch (type)
            {
                case FoodType.Pizza:
                    prefab = pizzaPrefab;
                    spawnPoint = pizzaSpawnPoint;
                    break;
                case FoodType.Hotdog:
                    prefab = hotdogPrefab;
                    spawnPoint = hotdogSpawnPoint;
                    break;
            }

            if (prefab != null && spawnPoint != null)
            {
                GameObject newFood = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                FoodScript foodScript = newFood.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    foodScript = newFood.AddComponent<FoodScript>();
                }
                // Set the food type
                foodScript.SetFoodType(type);
                Debug.Log($"Spawned new {type} at {spawnPoint.name}");
            }
            else
            {
                Debug.LogError($"Missing prefab or spawn point for {type}");
            }
        }

        private void Invoke(string methodName, float delay, FoodType type)
        {
            Invoke(() => SpawnFood(type), delay);
        }

        private void Invoke(System.Action action, float delay)
        {
            StartCoroutine(InvokeRoutine(action, delay));
        }

        private System.Collections.IEnumerator InvokeRoutine(System.Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
