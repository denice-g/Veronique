using UnityEngine;
using TMPro;
using System.Collections;

public class BedroomEntryNPC : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup npcCanvas;
    public TMP_Text npcText;
    public GameObject npcCharacter;

    [Header("Messages")]
    [TextArea(3, 5)]
    [Tooltip("Message shown when player enters room for the first time")]
    public string firstEntryMessage = "Welcome to the crisis room. Something urgent requires your attention...";

    [Header("Messages")]
    [TextArea(3, 5)]
    [Tooltip("Message shown when player enters room for the first time")]
    public string secondEntryMessage = "Welcome to the crisis room. Something urgent requires your attention...";

    [Header("Animation Settings")]
    [Range(0.5f, 5f)]
    public float fadeSpeed = 2f;

    [Range(2f, 10f)]
    public float messageDisplayDuration = 5f;

    public bool autoFadeAfterMessage = true;

    [Header("Audio (Optional)")]
    public AudioClip entrySound;
    public AudioClip interactionSound;
    private AudioSource audioSource;

    [Header("Room Tracking")]
    [Tooltip("Unique identifier for this room (e.g., 'Bedroom', 'CockpitLeft')")]
    public string roomID = "Bedroom";

    // Internal state
    private bool hasEnteredRoom = false;
    private bool isTransitioning = false;
    private string playerPrefsKey;

    public VictoryArrow victoryArrow;

    void Start()
    {
        // Validate UI
        if (!ValidateUIReferences())
        {
            Debug.LogError("[BedroomEntryNPC] Missing UI references!");
            enabled = false;
            return;
        }

        // Setup audio
        if (entrySound != null || interactionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Create unique PlayerPrefs key for this room
        playerPrefsKey = $"RoomVisited_{roomID}";

        // Initialize as hidden
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }

        // Auto-find VictoryArrow if not assigned
        if (victoryArrow == null)
        {
            victoryArrow = FindObjectOfType<VictoryArrow>();
        }

        // Check if player has visited this room before
        CheckFirstEntry();
    }

    private bool ValidateUIReferences()
    {
        if (npcCanvas == null)
        {
            Debug.LogError("[BedroomEntryNPC] NPC Canvas not assigned!");
            return false;
        }

        if (npcText == null)
        {
            Debug.LogError("[BedroomEntryNPC] NPC Text not assigned!");
            return false;
        }

        return true;
    }

    private void CheckFirstEntry()
    {
        // Check PlayerPrefs to see if room was visited before
        int visited = PlayerPrefs.GetInt(playerPrefsKey, 0);

        if (visited == 0)
        {
            // First time entering this room
            hasEnteredRoom = true;
            PlayerPrefs.SetInt(playerPrefsKey, 1);
            PlayerPrefs.Save();

            Debug.Log($"[BedroomEntryNPC] First entry to room: {roomID}");

            // Show entry messages
            StartCoroutine(ShowBothMessages());

        }
        else
        {
            Debug.Log($"[BedroomEntryNPC] Room already visited: {roomID}");
        }
    }

    private void ShowFirstEntryMessage()
    {
        if (isTransitioning) return;

        StartCoroutine(ShowMessage(firstEntryMessage, entrySound));
    }

    private void ShowSecondEntryMessage()
    {
        if (isTransitioning) return;

        StartCoroutine(ShowMessage(secondEntryMessage, entrySound));
    }

    private IEnumerator ShowBothMessages()
    {
        // Show first message
        yield return StartCoroutine(ShowMessage(firstEntryMessage, entrySound));
        
        // Wait a bit between messages (optional)
        yield return new WaitForSeconds(0.5f);
        
        // Show second message
        yield return StartCoroutine(ShowMessage(secondEntryMessage, entrySound));

        // Show victory arrow (no animation, just appears)
            if (victoryArrow != null)
            {
                victoryArrow.ShowVictoryArrow();
            }
    }


    private IEnumerator ShowMessage(string message, AudioClip sound = null)
    {
        isTransitioning = true;

        // Set message text
        if (npcText != null)
        {
            npcText.text = message;
        }

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Play sound
        PlaySound(sound);

        // Wait for display duration
        yield return new WaitForSeconds(messageDisplayDuration);

        // Fade out if auto-fade enabled
        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }

        isTransitioning = false;
    }

    private IEnumerator FadeIn()
    {
        if (npcCanvas == null) yield break;

        npcCanvas.interactable = true;
        npcCanvas.blocksRaycasts = true;

        while (npcCanvas.alpha < 1f)
        {
            npcCanvas.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        npcCanvas.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (npcCanvas == null) yield break;

        while (npcCanvas.alpha > 0f)
        {
            npcCanvas.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        npcCanvas.alpha = 0f;
        npcCanvas.interactable = false;
        npcCanvas.blocksRaycasts = false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    [ContextMenu("Test First Entry Message")]
    public void TestFirstEntry()
    {
        StartCoroutine(ShowBothMessages());
    }

    [ContextMenu("Reset Room Visit")]
    public void ResetRoomVisit()
    {
        PlayerPrefs.DeleteKey(playerPrefsKey);
        PlayerPrefs.SetInt(playerPrefsKey, 0);
        hasEnteredRoom = false;
        Debug.Log($"[BedroomEntryNPC] Reset room visit for: {roomID}");
    }

    public void FadeOutNow()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    public bool HasVisitedRoom()
    {
        return PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;
    }
}