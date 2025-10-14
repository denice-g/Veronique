using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    [Header("Retry Settings")]
    [Tooltip("Leave blank to reload the current scene.")]
    public string sceneToReload;

    [Header("Menu Settings")]
    [Tooltip("Scene name for the main menu (for the Quit button).")]
    public string mainMenuSceneName = "MainMenu"; // change this to your actual menu scene

    // Called by the Retry button
    public void Retry()
    {
        // Resume time (it was frozen)
        Time.timeScale = 1f;

        // Reload current or specific scene
        string target = string.IsNullOrEmpty(sceneToReload)
            ? SceneManager.GetActiveScene().name
            : sceneToReload;

        SceneManager.LoadScene(target);
    }

    // Called by the Quit button
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
