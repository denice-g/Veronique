using UnityEngine;

public class HingeConnector : MonoBehaviour, IWireConnectable
{
    public Transform connectionPoint;
    public Transform playerTransform;
    public WireBox connectedWireBox;
    public IWireConnectable previousConnectable;
    public bool isEndpoint = false;
    private SpriteRenderer cachedSprite;
    private bool playerInRange = false;
    public bool isConnected = false;

    // on trigger, if 'E' is pressed, connect wire
    private void Update()
    {
        // Can only connect, not start a wire from here
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (WireManager.Instance.isDragging)
            {
                if(isConnected)
                    return;
                WireManager.Instance.ConnectEndpoint(this);
            } else if(connectedWireBox != null && !connectedWireBox.isConnected && isEndpoint)
            {
                WireManager.Instance.StartWire(this, connectedWireBox, playerTransform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
        }
    }

    public Transform GetTransform() => transform;

    private void Awake()
    {
        cachedSprite = GetComponent<SpriteRenderer>();
    }

    public Vector3 GetConnectPoint()
    {
        if (connectionPoint != null)
            return connectionPoint.position;

        if (cachedSprite != null)
            return cachedSprite.bounds.center;

        return transform.position;
    }
}
