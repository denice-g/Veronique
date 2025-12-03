using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MenuScript : MonoBehaviour
{
    public static bool GameisPaused = false;
    private static bool IsMainMenu = true;

    [Header("---------- MenuUIs ----------")]
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject audioMenuUI;
    [SerializeField] private GameObject confirmMenuUI;
    [SerializeField] private GameObject timerUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameEndUI;
    [SerializeField] private VideoPlayer startVideo;

    [Header("---------- Buttons ----------")]
    [SerializeField] private GameObject confirmQuitButton;
    [SerializeField] private GameObject confirmExitButton;

    private float crisisDuration = 600f;
    public float TimeLeft;

    [Header("---------- Triggers ----------")]
    public bool InCrisis = false;
    public bool IsGameOver = false;
    public bool IsGameFinished = false;

    [Header("---------- Text ----------")]
    [SerializeField] private TextMeshProUGUI timerLabel;

    private string currentScene;
    public static MenuScript instance;

    private void Awake()
    {
        // Destroy duplicates
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize currentScene in case this object starts in a scene
        currentScene = SceneManager.GetActiveScene().name;
        StartCoroutine(CheckScene());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        StartCoroutine(CheckScene());
    }

    private IEnumerator CheckScene()
    {
        if (currentScene == "MainMenu")
        {
            // Close all menus
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            audioMenuUI.SetActive(false);
            confirmMenuUI.SetActive(false);
            confirmQuitButton.SetActive(false);
            confirmExitButton.SetActive(false);
            timerUI.SetActive(false);
            gameEndUI.SetActive(false);

            GameisPaused = false;
            Time.timeScale = 1f; // Ensure time is running
        }

        //Activate if player finishes game
        if (currentScene == "End_Screen")
        {
            // Close all menus
            mainMenuUI.SetActive(false);
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(false);
            audioMenuUI.SetActive(false);
            confirmMenuUI.SetActive(false);
            confirmQuitButton.SetActive(false);
            confirmExitButton.SetActive(false);
            timerUI.SetActive(false);

            InCrisis = false;

            //Delay the UI activation for a couple seconds
            yield return new WaitForSeconds(3);
            gameEndUI.SetActive(true);
            IsGameFinished = true;

            GameisPaused = false;
            Time.timeScale = 0f;
        }
    }

    void Update()
    {
        //Pause/Resume game everytime escape is clicked
        if (Input.GetKeyDown(KeyCode.Escape) && currentScene != "MainMenu")
        {
            if (GameisPaused)
            {
                //Makes it unable to pause when game over
                if (IsGameOver) return;

                Resume();
            }
            else
            {
                //Makes it unable to resume when game over
                if (IsGameOver) return;

                Pause();
            }
        }
        
        if(InCrisis)
        {
            TimeLeft -= Time.deltaTime;
            UpdateTimerLabel(TimeLeft);
        }

        if (TimeLeft <= 0f)
        {
            if (IsGameOver) return;
            TimeLeft = 0f;
            GameOver();
        }
    }

    public void StartGame()
    {
        IsMainMenu = false;
        IsGameFinished = false;
        mainMenuUI.SetActive(false);

        StartCoroutine(PlayIntroAndLoadScene());
        //SceneManager.LoadScene("Bedroom");
    }

    //Plays start game video then changes to bedroom scene
    private IEnumerator PlayIntroAndLoadScene()
    {
        // Show video panel
        startVideo.gameObject.SetActive(true);

        // Prepare video
        startVideo.Prepare();
        while (!startVideo.isPrepared)
            yield return null;

        // Play video
        startVideo.Play();

        // Wait for video to finish
        while (startVideo.isPlaying)
            yield return null;

        // Load scene while video is still visible
        SceneManager.LoadScene("Bedroom");

        // Wait one frame for scene to fully load
        yield return null;

        // NOW hide the video instantly
        startVideo.gameObject.SetActive(false);
    }

    public void Resume()
    {
        //Close all menus
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        confirmMenuUI.SetActive(false);
        confirmQuitButton.SetActive(false);
        confirmExitButton.SetActive(false);

        //Clear last used button to prevent continued button highlighting
        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1f;
        GameisPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;
    }

    //When options button is clicked
    public void Options()
    {
        if (IsMainMenu)
        {
            mainMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }
        else
        {
            pauseMenuUI.SetActive(false);
            optionsMenuUI.SetActive(true);
        }
            
    }

    //When audio button is clicked (in options menu)
    public void AudioSettings()
    {
        optionsMenuUI.SetActive(false);
        audioMenuUI.SetActive(true);
    }

    //When back button is clicked (if in options menu)
    public void ReturnToPauseMenu()
    {
        if (IsMainMenu)
        {
            optionsMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
        else
        {
            optionsMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
        }
        
    }

    //When back button is clicked (if in audio menu)
    public void ReturnToOptionsMenu()
    {
        audioMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    //When quit button is clicked
    public void QuitToMenu()
    {
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        confirmMenuUI.SetActive(true);
        confirmQuitButton.SetActive(true);
    }

    //When exit button is clicked
    public void ExitGame()
    {
        pauseMenuUI.SetActive(false);
        gameOverUI.SetActive(false);
        confirmMenuUI.SetActive(true);
        confirmExitButton.SetActive(true);
    }

    //When yes is clicked (for quit to menu)
    public void ConfirmQuit()
    {
        // Close all menus
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        audioMenuUI.SetActive(false);
        confirmMenuUI.SetActive(false);
        timerUI.SetActive(false);
        gameOverUI.SetActive(false);
        confirmQuitButton.SetActive(false);
        confirmExitButton.SetActive(false);

        //Unpause and set game events false
        Time.timeScale = 1f;
        GameisPaused = false;
        InCrisis = false;
        IsGameOver = false;
        
        //Reset timer
        TimeLeft = crisisDuration;

        //Open Main Menu
        IsMainMenu = true;
        mainMenuUI.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }

    //When yes is clicked (for exit game)
    public void ConfirmExit()
    {
        Application.Quit();

        //Exit game if in unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    //If no is clicked
    public void No()
    {

        if (IsGameOver)
        {
            confirmMenuUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
        else if(IsMainMenu)
        {
            confirmMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
        }
        else if (IsGameFinished)
        {
            confirmMenuUI.SetActive(false);
            gameEndUI.SetActive(true);
        }
        else
        {
            confirmMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
        }

        //Deactivate both confirm buttons
        confirmQuitButton.SetActive(false);
        confirmExitButton.SetActive(false);
    }

    public void TriggerCrisis()
    {
        CanvasGroup group = timerUI.GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = false;
        AudioManager.instance.PlayMusic(AudioManager.instance.gameStartMusic);

        if (InCrisis) return; // already running
        InCrisis = true;
        TimeLeft = crisisDuration;
        timerUI.SetActive(true);
        UpdateTimerLabel(TimeLeft);
    }
    
    void UpdateTimerLabel(float seconds)
    {
        if (!timerLabel) return;
        int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int m = s / 60;
        int sec = s % 60;
        timerLabel.text = $"{m:0}:{sec:00}";
    }
    
    private void GameOver()
    {
        IsGameOver = true;
        InCrisis = false;
        Time.timeScale = 0f;
        GameisPaused = true;
        gameOverUI.SetActive(true);
    }
}
