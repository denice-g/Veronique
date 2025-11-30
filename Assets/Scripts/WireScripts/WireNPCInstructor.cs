using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WireNPCInstructor : MonoBehaviour
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
    
    [Range(0.5f, 3f)]
    public float pauseBetweenMessages = 1f;

    [Header("Multi-Stage Instructions")]
    [TextArea(3, 5)]
    public string instructionPart1 = "Welcome to the wire connection puzzle. Your goal is to connect wires between the colored boxes.";
    
    [TextArea(3, 5)]
    public string instructionPart2 = "Match the wire colors to the box colors. Connect all three pairs to open the door. Drag from one box to another.";

    [TextArea(3, 5)]
    public string instructionPart3 = "Match the wire colors to the box colors. Connect all three pairs to open the door. Drag from one box to another.";

    [Header("Victory Message")]
    [TextArea(2, 4)]
    public string victoryMessage = "Excellent work! All wires connected correctly. The door is now open!";

    [Header("Audio (Optional)")]
    public AudioClip messageSound;
    public AudioClip hintSound;
    public AudioClip victorySound;
    private AudioSource audioSource;

    // Internal state
    private int resetCount = 0;
    private bool hasShownIntro = false;
    private bool hasShownHint = false;
    private bool isTransitioning = false;

    void Start()
    {
        // Validate UI
        if (!ValidateUIReferences())
        {
            Debug.LogError("[WireNPCInstructor] Missing UI references!");
            enabled = false;
            return;
        }

        // Setup audio
        if (messageSound != null || hintSound != null || victorySound != null)
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

        // Auto-start intro
        ShowIntroInstructions();
    }

    private bool ValidateUIReferences()
    {
        if (npcCanvas == null)
        {
            Debug.LogError("[WireNPCInstructor] NPC Canvas not assigned!");
            return false;
        }

        if (npcText == null)
        {
            Debug.LogError("[WireNPCInstructor] NPC Text not assigned!");
            return false;
        }

        return true;
    }

    public void ShowIntroInstructions()
    {
        if (hasShownIntro) return;
        
        hasShownIntro = true;
        StartCoroutine(MultiStageInstructionSequence());
    }

    private IEnumerator MultiStageInstructionSequence()
    {
        isTransitioning = true;

        // === STAGE 1: First instruction ===
        if (npcText != null)
        {
            npcText.text = instructionPart1;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(messageSound);

        Debug.Log("[WireNPCInstructor] Showing instruction part 1");

        yield return new WaitForSeconds(messageDisplayDuration);

        // Fade out between messages
        yield return StartCoroutine(FadeOut());

        // Pause between messages
        yield return new WaitForSeconds(pauseBetweenMessages);

        // === STAGE 2: Second instruction ===
        if (npcText != null)
        {
            npcText.text = instructionPart2;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(messageSound);

        Debug.Log("[WireNPCInstructor] Showing instruction part 2");

        yield return new WaitForSeconds(messageDisplayDuration);

        // Final fade out
        yield return StartCoroutine(FadeOut());

        // === STAGE 3: Second instruction ===
        if (npcText != null)
        {
            npcText.text = instructionPart3;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(messageSound);

        Debug.Log("[WireNPCInstructor] Showing instruction part 3");

        yield return new WaitForSeconds(messageDisplayDuration);

        // Final fade out
        yield return StartCoroutine(FadeOut());

        isTransitioning = false;
        Debug.Log("[WireNPCInstructor] Intro sequence complete");
    }

    private IEnumerator ShowHintSequence(string hintText)
    {
        isTransitioning = true;

        // Wait a moment after reset
        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = hintText;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintSound ?? messageSound);

        Debug.Log("[WireNPCInstructor] Showing hint");

        yield return new WaitForSeconds(messageDisplayDuration + 1f); // Show hint a bit longer

        yield return StartCoroutine(FadeOut());

        isTransitioning = false;
    }

    public void ShowVictoryMessage()
    {
        if (isTransitioning) return;
        
        StartCoroutine(ShowVictorySequence());
    }

    private IEnumerator ShowVictorySequence()
    {
        isTransitioning = true;

        // Brief delay for door animation
        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = victoryMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(victorySound ?? messageSound);

        Debug.Log("[WireNPCInstructor] Victory message displayed");

        // Keep victory message visible (don't auto-fade)
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

    public void FadeOutNow()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    public void Reset()
    {
        StopAllCoroutines();
        
        resetCount = 0;
        hasShownIntro = false;
        hasShownHint = false;
        isTransitioning = false;

        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }

        Debug.Log("[WireNPCInstructor] Reset complete");
    }
}