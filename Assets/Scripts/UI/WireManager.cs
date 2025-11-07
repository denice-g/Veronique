using UnityEngine;

public class WireManager : MonoBehaviour
{
    public static WireManager Instance;

    private LineRenderer currentWire;
    private Wirebox startBox;
    private bool isDragging = false;
    private Transform playerTransform;
    public bool InstanceDragging => isDragging;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isDragging && currentWire != null && playerTransform != null)
        {
            // Update the end of the wire to follow the player
            currentWire.SetPosition(1, playerTransform.position);
        }
    }

    public void StartWire(Wirebox box, Transform player)
    {
        if (isDragging) return; // Already dragging a wire

        startBox = box;
        playerTransform = player;
        isDragging = true;

        // Create a new wire
        GameObject wireObj = new GameObject("Wire");
        currentWire = wireObj.AddComponent<LineRenderer>();

        // Setup line appearance
        currentWire.positionCount = 2;
        currentWire.startWidth = 0.05f;
        currentWire.endWidth = 0.05f;
        currentWire.material = new Material(Shader.Find("Sprites/Default"));
        currentWire.startColor = Color.yellow;
        currentWire.endColor = Color.yellow;

        // Start and end positions
        currentWire.SetPosition(0, box.transform.position);
        currentWire.SetPosition(1, player.position);
    }

    public void TryConnectWire(Wirebox targetBox)
    {
        if (!isDragging || currentWire == null || startBox == null) return;

        // Prevent connecting to the same box
        if (targetBox == startBox) return;

        // Complete connection
        currentWire.SetPosition(1, targetBox.transform.position);

        startBox.OnConnected(targetBox);
        targetBox.OnConnected(startBox);

        // Reset dragging state
        isDragging = false;
        startBox = null;
        currentWire = null;
        playerTransform = null;
    }

    public void CancelWire()
    {
        if (currentWire != null)
            Destroy(currentWire.gameObject);

        isDragging = false;
        startBox = null;
        currentWire = null;
        playerTransform = null;
    }
}
