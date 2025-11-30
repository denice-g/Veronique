using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// NPC instructor for the lever sequence puzzle
/// Shows intro instructions, provides hints after failures, and celebrates victory
/// </summary>
public class LeverNPCInstructor : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup npcCanvas;
    public TMP_Text npcText;
    public GameObject npcCharacter;

    [Header("Animation Settings")]
    [Range(0.5f, 5f)]
    public float fadeSpeed = 2f;
    
    [Range(2f, 10f)]
    public float messageDisplayDuration = 5f;
    
    public bool autoFadeAfterMessage = true;

    [Header("Dialogue Messages")]
    [TextArea(3, 5)]
    public string introMessage = "Welcome to the power room. Pull the levers in the correct sequence to restore power. Look at the conduit lights for clues.";

    [TextArea(2, 4)]
    public string hintMessageTemplate = "Having trouble? Try starting with lever {0} - it should be pulled first.";

    [TextArea(2, 4)]
    public string victoryMessage = "Excellent! Power restored! The lights are back on. Proceed to the next area.";

    [Header("Audio (Optional)")]
    public AudioClip hintAppearSound;
    public AudioClip victorySound;
    private AudioSource audioSource;

    private bool isTransitioning = false;
    private Coroutine activeCoroutine;
    private bool hasShownIntro = false;
    private bool hasShownHint = false;

    void Start()
    {
        // Validate UI
        if (!ValidateUIReferences())
        {
            Debug.LogError("[LeverNPCInstructor] Missing UI references!");
            enabled = false;
            return;
        }

        // Setup audio
        if (hintAppearSound != null || victorySound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // Initialize as hidden
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }
    }

    private bool ValidateUIReferences()
    {
        if (npcCanvas == null)
        {
            Debug.LogError("[LeverNPCInstructor] NPC Canvas not assigned!");
            return false;
        }

        if (npcText == null)
        {
            Debug.LogError("[LeverNPCInstructor] NPC Text not assigned!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Show intro instructions when puzzle starts
    /// </summary>
    public void ShowIntroInstructions()
    {
        if (hasShownIntro) return;
        
        hasShownIntro = true;
        
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowMessage(introMessage));
    }

    /// <summary>
    /// Show hint after failed attempts
    /// </summary>
    public void ShowHint(string firstLeverID)
    {
        if (hasShownHint) return; // Only show hint once
        
        hasShownHint = true;
        
        string hintMessage = string.Format(hintMessageTemplate, firstLeverID);
        
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowMessage(hintMessage, hintAppearSound));
    }

    /// <summary>
    /// Show victory message when puzzle is solved
    /// </summary>
    public void ShowVictoryMessage()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        activeCoroutine = StartCoroutine(ShowMessage(victoryMessage, victorySound, false)); // Don't auto-fade victory
    }

    /// <summary>
    /// Generic message display coroutine
    /// </summary>
    private IEnumerator ShowMessage(string message, AudioClip sound = null, bool autoFade = true)
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
        PlaySound(sound ?? hintAppearSound);

        Debug.Log($"[LeverNPCInstructor] Showing: {message}");

        // Wait for display duration
        yield return new WaitForSeconds(messageDisplayDuration);

        // Fade out if auto-fade enabled
        if (autoFade && autoFadeAfterMessage)
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

    /// <summary>
    /// Manually fade out the NPC
    /// </summary>
    public void FadeOutNow()
    {
        if (activeCoroutine != null) StopCoroutine(activeCoroutine);
        StartCoroutine(FadeOut());
    }

    /// <summary>
    /// Reset NPC state (for puzzle retry)
    /// </summary>
    public void Reset()
    {
        StopAllCoroutines();
        
        hasShownIntro = false;
        hasShownHint = false;
        isTransitioning = false;

        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }

        Debug.Log("[LeverNPCInstructor] Reset complete");
    }
}