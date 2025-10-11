using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScript : MonoBehaviour
{
    private string nextScene = "Bedroom";

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
        SceneManager.LoadScene(nextScene);
    }
}
