using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    [Header("---------- Buttons ----------")]
    [SerializeField] private GameObject confirmQuitButton;
    [SerializeField] private GameObject confirmExitButton;

    private float crisisDuration = 60f;
    public float TimeLeft;

    [Header("---------- Triggers ----------")]
    public bool InCrisis = false;
    public bool isGameOver = false;

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
        CheckScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.name;
        CheckScene();
    }

    private void CheckScene()
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

            GameisPaused = false;
            Time.timeScale = 1f; // Ensure time is running
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
                if (isGameOver) return;

                Resume();
            }
            else
            {
                //Makes it unable to resume when game over
                if (isGameOver) return;

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
            if (isGameOver) return;
            TimeLeft = 0f;
            GameOver();
        }
    }

    public void StartGame()
    {
        IsMainMenu = false;
        mainMenuUI.SetActive(false);
        SceneManager.LoadScene("Bedroom");
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
        isGameOver = false;
        
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

        if (isGameOver)
        {
            confirmMenuUI.SetActive(false);
            gameOverUI.SetActive(true);
        }
        else if(IsMainMenu)
        {
            confirmMenuUI.SetActive(false);
            mainMenuUI.SetActive(true);
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
        isGameOver = true;
        InCrisis = false;
        Time.timeScale = 0f;
        gameOverUI.SetActive(true);
    }
}
