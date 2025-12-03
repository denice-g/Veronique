using UnityEngine;
using UnityEngine.SceneManagement;

public class Puzzle2ButtonScript : MonoBehaviour
{
    private bool playerInRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            //PuzzlesScript.Instance.OpenPuzzle2();
            SceneManager.LoadScene("End_Screen");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }
}
