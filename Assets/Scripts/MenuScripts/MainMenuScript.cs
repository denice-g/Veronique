using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene("Bedroom");
    }

    public void exitGame()
    {
        Application.Quit();

        //Exit game if in unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
