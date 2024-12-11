using UnityEngine;

namespace SojaExiles
{
    public class BackgroundMusic : MonoBehaviour
    {
        public static BackgroundMusic Instance { get; private set; }
        
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
        
        private AudioSource audioSource;

        private void Awake()
        {
            // Singleton pattern to keep music playing between scenes
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // Setup audio source
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = backgroundMusic;
                audioSource.volume = volume;
                audioSource.loop = true;
                audioSource.playOnAwake = true;
                audioSource.Play();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (audioSource != null)
            {
                audioSource.volume = volume;
            }
        }

        public void ToggleMusic(bool enable)
        {
            if (audioSource != null)
            {
                if (enable && !audioSource.isPlaying)
                    audioSource.Play();
                else if (!enable && audioSource.isPlaying)
                    audioSource.Pause();
            }
        }
    }
}
