using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    //Have an effect on button when hovered over
    /*private void OnMouseEnter()
    {
        
    }

    //Go back to regular button effect
    private void OnMouseExit()
    {

    }*/

    //Load scene on mouse click
    private void OnMouseDown()
    {
        Application.Quit();

        //Exit game if in unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
