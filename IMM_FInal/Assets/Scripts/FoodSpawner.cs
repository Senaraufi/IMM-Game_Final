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
        [SerializeField] private Vector3 spawnOffset = new Vector3(0, 0.5f, 0); // Offset to spawn above the counter

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
            // Ensure we have all required components
            if (pizzaPrefab == null || hotdogPrefab == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing food prefabs!");
                return;
            }

            if (pizzaSpawnPoint == null || hotdogSpawnPoint == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing spawn points!");
                return;
            }

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
                Instance.Invoke(() => Instance.SpawnFood(type), Instance.spawnDelay);
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
                // Spawn the food slightly above the spawn point to prevent clipping
                Vector3 spawnPosition = spawnPoint.position + spawnOffset;
                GameObject newFood = Instantiate(prefab, spawnPosition, spawnPoint.rotation);
                
                // Make sure it has a FoodScript component
                FoodScript foodScript = newFood.GetComponent<FoodScript>();
                if (foodScript == null)
                {
                    foodScript = newFood.AddComponent<FoodScript>();
                }
                
                // Set the food type
                foodScript.SetFoodType(type);
                
                // Add Rigidbody if it doesn't exist
                Rigidbody rb = newFood.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = newFood.AddComponent<Rigidbody>();
                    rb.useGravity = true;
                    rb.isKinematic = false;
                }
                
                // Add BoxCollider if it doesn't exist
                BoxCollider col = newFood.GetComponent<BoxCollider>();
                if (col == null)
                {
                    col = newFood.AddComponent<BoxCollider>();
                }
                
                Debug.Log($"Spawned new {type} at {spawnPoint.name}");
            }
            else
            {
                Debug.LogError($"Missing prefab or spawn point for {type}");
            }
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
