using UnityEngine;

public class WireManager : MonoBehaviour
{
    public static WireManager Instance;

    public LineRenderer wire;         // The visible wire
    public Transform playerTransform; // The player transform to follow when dragging
    public LayerMask obstructionMask; // set in Inspector to include default and Wire layers

    private IWireConnectable startPoint;
    private IWireConnectable endPoint;
    private IWireConnectable connectPoint;

    public bool isDragging = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isDragging && playerTransform != null)
        {
            // Update the end of the wire to follow the player
            wire.SetPosition(1, playerTransform.position);
        }
    }

    public void StartWire(IWireConnectable connector, WireBox start, Transform player) // Wire always starts from a WireBox
    {
        startPoint = start;
        connectPoint = connector;
        playerTransform = player;

        // Initialize line
        GameObject wireObj = new GameObject("Wire");
        wire = wireObj.AddComponent<LineRenderer>();
        wire.positionCount = 2;
        wire.material = new Material(Shader.Find("Sprites/Default"));

        // set color to wirebox color
        if (ColorUtility.TryParseHtmlString(start.color, out Color wireColor))
        {
            wire.startColor = wireColor;
            wire.endColor = wireColor;
        }
        else
        {
            wire.startColor = Color.green;
            wire.endColor = Color.green;
        }

        wire.startWidth = 0.2f;
        wire.endWidth = 0.2f;

        //dragPoint.position = start.GetConnectPoint();
        wire.SetPosition(0, connector.GetConnectPoint());
        wire.SetPosition(1, playerTransform.position);

        isDragging = true;
    }

    public void ConnectEndpoint(IWireConnectable end)
    {
        if(end == connectPoint)
        {
            Destroy(wire.gameObject);
            isDragging = false;
            return;
        }
        if(end.isWireBox && startPoint.isWireBox)
        {
            // if its the same wirebox, cancel the wire
            if(end == startPoint)
            {
                Destroy(wire.gameObject);
                isDragging = false;
                return;
            }
            // make sure that the two wireboxes are the same color - do nothing if different
            if(((WireBox)end).color != ((WireBox)startPoint).color)
            {
                return;
            } else {
                // mark both wireboxes as connected
                ((WireBox)end).isConnected = true;
                ((WireBox)startPoint).isConnected = true;
            }
        }

        // Finish the wire
        endPoint = end;
        isDragging = false;

        wire.SetPosition(1, end.GetConnectPoint());

        bool validConnection = true;

        // Check along the length of the wire if it intersects with any colliders
        // excluding trigger colliders such as the player and wireboxes
        RaycastHit2D[] hits = Physics2D.LinecastAll(connectPoint.GetConnectPoint(), end.GetConnectPoint(), obstructionMask);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("Wire hit: " + hit.collider.name);

            // dont ignore if the trigger is a wirecollider - will have layer "Wire"
            if((hit.collider.isTrigger || hit.collider.CompareTag("Player")) && !(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wire")))
            {
                continue;
            } else {
                // hit a non-trigger collider, invalid connection
                Debug.Log("Wire connection invalid due to obstacle: " + hit.collider.name);
                // Destroy wire
                if(end.isWireBox){
                    ((WireBox)end).isConnected = false;
                    ((WireBox)startPoint).isConnected = false;
                }
                Destroy(wire.gameObject);
                validConnection = false;
                return;
            }
        }

        if(!validConnection)
            return;

        if (!end.isWireBox && startPoint.isWireBox)
        {
            // if connecting from wirebox to hinge, set the hinge's connectedWireBox
            HingeConnector hinge = (HingeConnector)end;
            hinge.connectedWireBox = (WireBox)startPoint;
        }

        if(!end.isWireBox)
        {
            HingeConnector hinge = (HingeConnector)end;
            hinge.previousConnectable = connectPoint;
            hinge.isEndpoint = true;
            hinge.isConnected = true;
        }

        if(!connectPoint.isWireBox)
        {
            HingeConnector hinge = (HingeConnector)connectPoint;
            hinge.isEndpoint = false;
        }

        // Finally, add a trigger collider to the wire for future obstruction checks
        float segmentRadius = 0.1f; // adjust as needed 

        GameObject segObj = new GameObject("WireCollider");
        var col = segObj.AddComponent<BoxCollider2D>();
        // Make wire collider shorter by 1.5f than the wire to avoid overlap issues
        col.size = new Vector2(Vector3.Distance(connectPoint.GetConnectPoint(), end.GetConnectPoint()) - 1.5f , segmentRadius * 2);
        col.isTrigger = true;

        // Position and scale the collider to match the wire - shave off excess length to avoid overlap issues
        segObj.transform.position = (connectPoint.GetConnectPoint() + end.GetConnectPoint()) / 2;
        Vector3 direction = end.GetConnectPoint() - connectPoint.GetConnectPoint();
        segObj.transform.right = direction.normalized;

        segObj.layer = LayerMask.NameToLayer("Wire");

        connectPoint = null;
        startPoint = null;

    }
}
