using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Simple NPC that fades in, displays a message, then fades out
/// Perfect for giving instructions at the start of a scene/game
/// </summary>
public class NPCHintManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The Canvas Group containing the NPC panel")]
    public CanvasGroup npcCanvas;
    
    [Tooltip("The text component for NPC dialogue")]
    public TMP_Text npcText;
    
    [Tooltip("Optional: NPC character image")]
    public GameObject npcCharacter;

    [Header("Instruction Settings")]
    [TextArea(3, 6)]
    [Tooltip("The message the NPC will display")]
    public string instructionMessage = "Welcome! Here are your instructions...";
    
    [Tooltip("How long to display the message (in seconds)")]
    [Range(2f, 20f)]
    public float displayDuration = 5f;
    
    [Tooltip("Delay before NPC appears (in seconds)")]
    [Range(0f, 5f)]
    public float initialDelay = 1f;

    [Header("Animation Settings")]
    [Tooltip("How fast the fade in/out animation is")]
    [Range(0.5f, 5f)]
    public float fadeSpeed = 2f;
    
    [Tooltip("Auto-start on scene load")]
    public bool autoStart = true;

    [Header("Audio (Optional)")]
    public AudioClip appearSound;
    public AudioClip disappearSound;
    private AudioSource audioSource;

    private bool hasPlayed = false;

    void Start()
    {
        // Setup audio
        if (appearSound != null || disappearSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Initialize NPC as hidden
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }

        // Auto-start if enabled
        if (autoStart)
        {
            ShowInstructions();
        }
    }

    /// <summary>
    /// Call this to show the NPC instructions
    /// Can be called from other scripts or events
    /// </summary>
    public void ShowInstructions()
    {
        if (hasPlayed) return; // Only show once
        
        hasPlayed = true;
        StartCoroutine(InstructionSequence());
    }

    /// <summary>
    /// Main instruction sequence: Fade in → Display → Fade out
    /// </summary>
    private IEnumerator InstructionSequence()
    {
        // Initial delay
        yield return new WaitForSeconds(initialDelay);

        // Set the instruction text
        if (npcText != null)
        {
            npcText.text = instructionMessage;
        }

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Play appear sound
        PlaySound(appearSound);

        // Display message for duration
        yield return new WaitForSeconds(displayDuration);

        // Play disappear sound
        PlaySound(disappearSound);

        // Fade out
        yield return StartCoroutine(FadeOut());

        Debug.Log("[NPCHintManager] Instructions complete!");
    }

    /// <summary>
    /// Fade in animation
    /// </summary>
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

    /// <summary>
    /// Fade out animation
    /// </summary>
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

    /// <summary>
    /// Reset so instructions can be shown again
    /// </summary>
    public void Reset()
    {
        hasPlayed = false;
        
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Manually fade out immediately (skip display duration)
    /// </summary>
    public void FadeOutNow()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }
}