using UnityEngine;

namespace SojaExiles
{
    public class SmokeEffectManager : MonoBehaviour
    {
        [Header("Smoke Effect Settings")]
        public GameObject smokeEffectPrefab; // Assign your smoke effect prefab here
        public Transform spawnPoint; // Where the smoke should appear
        private ParticleSystem activeSmoke;

        void Start()
        {
            // If spawnPoint is not set, use this object's position
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
        }

        public void PlaySmokeEffect()
        {
            if (smokeEffectPrefab != null)
            {
                // If there's already an active smoke effect, destroy it
                if (activeSmoke != null)
                {
                    Destroy(activeSmoke.gameObject);
                }

                // Instantiate the new smoke effect
                GameObject smokeObj = Instantiate(smokeEffectPrefab, spawnPoint.position, Quaternion.identity);
                activeSmoke = smokeObj.GetComponent<ParticleSystem>();

                if (activeSmoke != null)
                {
                    // Start the particle system
                    activeSmoke.Play();

                    // Automatically destroy the smoke effect after it's finished
                    float duration = activeSmoke.main.duration;
                    Destroy(smokeObj, duration);
                }
                else
                {
                    Debug.LogWarning("Smoke effect prefab does not contain a ParticleSystem component!");
                }
            }
            else
            {
                Debug.LogWarning("No smoke effect prefab assigned!");
            }
        }

        public void StopSmokeEffect()
        {
            if (activeSmoke != null)
            {
                // Stop emitting new particles
                activeSmoke.Stop();
                
                // Destroy the game object after all particles have died
                float duration = activeSmoke.main.duration + activeSmoke.main.startLifetime.constantMax;
                Destroy(activeSmoke.gameObject, duration);
                activeSmoke = null;
            }
        }
    }
}
