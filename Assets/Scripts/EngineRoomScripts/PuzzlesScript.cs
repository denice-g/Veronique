using UnityEngine;

public class PuzzlesScript : MonoBehaviour
{
    [Header("---------- MenuUIs ----------")]
    [SerializeField] private GameObject puzzle1UI;
    [SerializeField] private GameObject puzzle2UI;

    public static PuzzlesScript Instance;
    public Puzzle1ButtonScript inPuzzle;
    public bool isPuzzleOpen = false;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPuzzleOpen)
        {
            puzzle1UI.SetActive(false);
            puzzle2UI.SetActive(false);

            if (inPuzzle != null)
            {
                inPuzzle.inPuzzle1 = false;
                inPuzzle.ReenableButton(); // re-enable the button
            }

            isPuzzleOpen = false;
        }
    }

    public void OpenPuzzle1()
    {
        puzzle1UI.SetActive(true);
    }

    public void OpenPuzzle2()
    {
        puzzle2UI.SetActive(true);
    }
}
