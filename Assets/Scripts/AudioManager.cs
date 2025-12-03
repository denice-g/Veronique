using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip mainMenuMusic;
    public AudioClip bedroomMusic;
    public AudioClip gameStartMusic;
    public AudioClip jump;
    public AudioClip walk;
    public AudioClip death;
    public AudioClip meow;

    public static AudioManager instance;

    private bool hasPlayedBedroomMusic = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
            
        DontDestroyOnLoad(gameObject);

        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusic(mainMenuMusic);
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Automatically switch background music based on scene name
        switch (scene.name)
        {
            case "MainMenu":
                hasPlayedBedroomMusic = false;

                if (musicSource.clip != mainMenuMusic)
                    PlayMusic(mainMenuMusic);
                break;

            case "Bedroom":
                if (!hasPlayedBedroomMusic)
                {
                    PlayMusic(bedroomMusic);
                    hasPlayedBedroomMusic = true;
                }

                break;

            default:
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void playerSFX(AudioClip clip)
    {
        if (clip == null) return;

        SFXSource.PlayOneShot(clip);
    }
}
