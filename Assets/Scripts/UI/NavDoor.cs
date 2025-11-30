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
    }

    void Update()
    {
        if (!doorOpened && wireBox1.isConnected && wireBox2.isConnected && wireBox3.isConnected)
        {
            OpenDoor();

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
