using UnityEngine;

public class InteractButton : MonoBehaviour
{
    [Tooltip("Tag used to detect the player.")]
    public string playerTag = "Player";

    [Tooltip("Only allow triggering once per scene (optional).")]
    public bool oneShot = true;

    [Tooltip("Optional prompt UI (e.g., 'Press E to start crisis').")]
    public GameObject interactPrompt;

    bool _playerInRange;
    bool _used;

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
            ShipCrisisController.Instance?.TriggerCrisis();
            _used = true;
            if (interactPrompt) interactPrompt.SetActive(false);
        }
    }
}
