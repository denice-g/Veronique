using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public static bool GameisPaused = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        //Pause/Resume game everytime escape is clicked
        if (Input.GetKeyDown(KeyCode.Escape))
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
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameisPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameisPaused = true;
    }

    //When quit button clicked
    public void quitToMenu()
    {
        Time.timeScale = 1f;
        GameisPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    //When exit button clicked
    public void exitGame()
    {
        Application.Quit();

        //Exit game if in unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
