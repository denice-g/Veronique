using UnityEngine;

/// <summary>
/// Makes 2D/3D objects or world-space UI clickable
/// Works perfectly for 2D games with sprites
/// Attach to Calendar and Key if they're sprites in the game world
/// </summary>
public class WorldSpaceClickable : MonoBehaviour
{
    public enum ObjectType { Calendar, Key }
    public ObjectType objectType;

    [Header("2D/3D Mode")]
    [Tooltip("Check this if your game is 2D (uses sprites and 2D colliders)")]
    public bool is2DGame = true;

    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E; // Key to press when near
    public bool requireProximity = false; // If true, player must be near to click
    public float interactionDistance = 3f;

    [Header("Visual Feedback")]
    public bool enableGlow = true;
    public Color glowColor = Color.yellow;
    public GameObject glowEffect; // Optional glow sprite/particle

    [Header("Optional Popup")]
    public GameObject popupPanel;

    private StoragePuzzleManager puzzleManager;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool hasBeenClicked = false;
    private bool playerInRange = false;

    void Start()
    {
        // Find puzzle manager
        puzzleManager = FindObjectOfType<StoragePuzzleManager>();
        
        if (puzzleManager == null)
        {
            Debug.LogError($"[WorldSpaceClickable] StoragePuzzleManager not found!");
        }

        // Find player
        player = GameObject.FindGameObjectWithTag("Player");

        // Get sprite renderer if available
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Ensure collider exists based on 2D/3D mode
        if (is2DGame)
        {
            if (GetComponent<Collider2D>() == null)
            {
                // Add a 2D box collider for sprites
                BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
                Debug.Log($"[WorldSpaceClickable] Added BoxCollider2D to {gameObject.name}");
            }
        }
        else
        {
            if (GetComponent<Collider>() == null)
            {
                // Add a 3D box collider
                gameObject.AddComponent<BoxCollider>();
                Debug.Log($"[WorldSpaceClickable] Added BoxCollider to {gameObject.name}");
            }
        }

        // Hide popup initially
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        // Hide glow initially
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
        }

        Debug.Log($"[WorldSpaceClickable] {objectType} initialized in {(is2DGame ? "2D" : "3D")} mode");
    }

    void Update()
    {
        // Check proximity if required
        if (requireProximity && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            playerInRange = distance <= interactionDistance;

            // Show/hide glow based on proximity
            if (glowEffect != null)
            {
                glowEffect.SetActive(playerInRange);
            }
        }
        else
        {
            playerInRange = true; // Always in range if proximity not required
        }

        // Check for key press interaction
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            OnObjectClicked();
        }
    }

    // Called when object is clicked (mouse click)
    // Works for both 2D and 3D
    void OnMouseDown()
    {
        if (requireProximity && !playerInRange)
        {
            Debug.Log($"[WorldSpaceClickable] Too far from {objectType}");
            return;
        }

        OnObjectClicked();
    }

    // Handle hover effect - works for both 2D and 3D
    void OnMouseEnter()
    {
        if (!playerInRange && requireProximity) return;

        Debug.Log($"[WorldSpaceClickable] Mouse entered {objectType}");

        if (enableGlow && spriteRenderer != null)
        {
            spriteRenderer.color = glowColor;
        }

        if (glowEffect != null)
        {
            glowEffect.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        Debug.Log($"[WorldSpaceClickable] Mouse exited {objectType}");

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (!requireProximity && glowEffect != null)
        {
            glowEffect.SetActive(false);
        }
    }

    private void OnObjectClicked()
    {
        if (puzzleManager == null)
        {
            Debug.LogError("[WorldSpaceClickable] Cannot click - puzzleManager is null!");
            return;
        }

        Debug.Log($"[WorldSpaceClickable] {objectType} clicked!");

        hasBeenClicked = true;

        // Show popup if assigned
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }

        // Notify puzzle manager
        switch (objectType)
        {
            case ObjectType.Calendar:
                puzzleManager.OnCalendarClicked();
                break;
            
            case ObjectType.Key:
                puzzleManager.OnKeyClicked();
                break;
        }
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public bool HasBeenClicked()
    {
        return hasBeenClicked;
    }

    public void ResetClickState()
    {
        hasBeenClicked = false;
    }

    // Draw interaction range in editor
    void OnDrawGizmosSelected()
    {
        if (requireProximity)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}