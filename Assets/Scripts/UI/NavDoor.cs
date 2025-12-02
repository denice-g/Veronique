using UnityEngine;

public class NavDoor : MonoBehaviour
{
    [SerializeField] private WireBox wireBox1;
    [SerializeField] private WireBox wireBox2;
    [SerializeField] private WireBox wireBox3;
    [SerializeField] private GameObject doorVisual;
    
    [Header("NPC Integration")]
    [SerializeField] private WireNPCInstructor npcInstructor;

    public VictoryArrow victoryArrow;
    
    private bool doorOpened = false;

    void Start()
    {
        // Auto-find NPC if not assigned
        if (npcInstructor == null)
        {
            npcInstructor = FindObjectOfType<WireNPCInstructor>();
        }

        // Auto-find VictoryArrow if not assigned
        if (victoryArrow == null)
        {
            victoryArrow = FindObjectOfType<VictoryArrow>();
        }

        if (GameManager.Instance != null && 
        GameManager.Instance.IsPuzzleComplete("WirePuzzle"))
        {
            doorOpened = true;
            
            // Set door to open state immediately
            if (doorVisual != null)
            {
                doorVisual.transform.Translate(1, 0, 0);
                doorVisual.transform.Rotate(0, 60, 0);
            }
            
            // Mark all wireboxes as connected (visual state)
            if (wireBox1 != null) wireBox1.isConnected = true;
            if (wireBox2 != null) wireBox2.isConnected = true;
            if (wireBox3 != null) wireBox3.isConnected = true;
            
            Debug.Log("[WirePuzzle] Already complete - door open");
        }
    }

    void Update()
    {
        if (!doorOpened && wireBox1.isConnected && wireBox2.isConnected && wireBox3.isConnected)
        {

            GameManager.Instance?.CompletePuzzle("WirePuzzle");

            if (GameManager.Instance.AreAllMainPuzzlesComplete())
            {
                OpenDoor();
            }

            // Show victory arrow (no animation, just appears)
            if (victoryArrow != null)
            {
                victoryArrow.ShowVictoryArrow();
            }
        }
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log("NavDoor opened!");

        if (doorVisual != null)
        {
            // rotate the door 60 degrees in Y
            // move the door to the right by 1 unit
            doorVisual.transform.Translate(1, 0, 0);
            doorVisual.transform.Rotate(0, 60, 0);
        }

        // Notify NPC of victory
        if (npcInstructor != null)
        {
            npcInstructor.ShowVictoryMessage();
        }

    }
}
