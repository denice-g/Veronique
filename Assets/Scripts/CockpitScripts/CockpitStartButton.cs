using UnityEngine;

public class CockpitStartButton : MonoBehaviour
{
    [Tooltip("Tag used to detect the player.")]
    public string playerTag = "Player";

    [Tooltip("Only allow triggering once per scene (optional).")]
    public bool oneShot = true;

    [Tooltip("Optional prompt UI (e.g., 'Press E to start crisis').")]
    public GameObject interactPrompt;

    [Header("NPC Integration")]
    [Tooltip("NPC that shows message after interaction")]
    public RoomEntryNPC roomNPC;

    bool _playerInRange;
    bool _used;

    void Start()
    {
        // Auto-find RoomEntryNPC if not assigned
        if (roomNPC == null)
        {
            roomNPC = FindObjectOfType<RoomEntryNPC>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_used && oneShot) return;
        if (other.CompareTag(playerTag))
        {
            _playerInRange = true;
            if (interactPrompt) interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
            if (interactPrompt) interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        if (_used && oneShot) return;
        if (!_playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // Trigger the crisis/menu
            MenuScript.instance?.TriggerCrisis();

            // Mark as used
            _used = true;

            // Hide prompt
            if (interactPrompt) interactPrompt.SetActive(false);

            // NEW: Notify NPC of interaction
            if (roomNPC != null)
            {
                roomNPC.ShowInteractionMessage();
            }

            Debug.Log("[InteractButton] Button interacted - NPC notified");
        }
    }
}