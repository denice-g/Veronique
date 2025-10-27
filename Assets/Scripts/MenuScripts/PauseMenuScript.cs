using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class PauseScript : MonoBehaviour
{
    public static bool GameisPaused = false;

    [Header("---------- MenuUIs ----------")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;
    [SerializeField] private GameObject audioMenuUI;
    [SerializeField] private GameObject confirmMenuUI;

    [Header("---------- Buttons ----------")]
    [SerializeField] private GameObject confirmQuitButton;
    [SerializeField] private GameObject confirmExitButton;

    private string currentScene;
    private static PauseScript instance;

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
            if(GameisPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
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

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;
    }
    
    //When options button is clicked
    public void Options()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    //When audio button is clicked (in options menu)
    public void AudioSettings()
    {
        optionsMenuUI.SetActive(false);
        audioMenuUI.SetActive(true);
    }

    //When back button is clicked (if in audio menu)
    public void ReturnToPauseMenu()
    {
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    //When back button is clicked (if in options menu)
    public void ReturnToOptionsMenu()
    {
        audioMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    //When quit button is clicked
    public void QuitToMenu()
    {
        pauseMenuUI.SetActive(false);
        confirmMenuUI.SetActive(true);
        confirmQuitButton.SetActive(true);
    }

    //When exit button is clicked
    public void ExitGame()
    {
        pauseMenuUI.SetActive(false);
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
        confirmQuitButton.SetActive(false);
        confirmExitButton.SetActive(false);



        Time.timeScale = 1f;
        GameisPaused = false;
        //Object.Destroy(gameObject);
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
        confirmMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        //Deactivate both confirm buttons
        confirmQuitButton.SetActive(false);
        confirmExitButton.SetActive(false);
    }
}
