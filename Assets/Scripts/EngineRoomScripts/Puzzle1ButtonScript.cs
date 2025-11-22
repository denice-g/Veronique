using UnityEngine;

public class Puzzle1ButtonScript : MonoBehaviour
{
    public GameObject panelToOpen; // Assign Panel1
    private bool playerInRange;

    public bool inPuzzle1 = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange) return;

        // If the puzzle is currently open, don't allow opening again
        if (PuzzlesScript.Instance != null && PuzzlesScript.Instance.isPuzzleOpen)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            PuzzlesScript.Instance.inPuzzle = this;
            PuzzlesScript.Instance.OpenPuzzle1();

            inPuzzle1 = true;
            PuzzlesScript.Instance.isPuzzleOpen = true;

            // disable this button's collider so it won't detect the same frame input
            var col = GetComponent<Collider2D>();
            if (col != null)
                col.enabled = false;

            // OR disable this entire script:
            // this.enabled = false;
        }
    }

    public void ReenableButton()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        this.enabled = true;
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
