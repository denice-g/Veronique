using UnityEngine;
using TMPro;
using System.Collections;

public class PuzzleStateManager : MonoBehaviour
{
    [Header("Puzzle Identity")]
    [Tooltip("Must match the name in GameManager (e.g., 'WordPuzzle', 'LeverPuzzle')")]
    public string puzzleID = "WordPuzzle";

    [Header("Completion Visual State")]
    [Tooltip("Objects to show when puzzle is already complete")]
    public GameObject[] completedStateObjects;
    
    [Tooltip("Objects to hide when puzzle is already complete")]
    public GameObject[] incompleteStateObjects;

    [Header("NPC Message")]
    public bool showCompletionMessage = true;
    public CanvasGroup npcCanvas;
    public TMP_Text npcText;
    
    [TextArea(3, 5)]
    public string alreadyCompletedMessage = "You've already completed this puzzle! Well done. The door remains open.";
    
    [Range(2f, 10f)]
    public float messageDisplayDuration = 5f;
    
    [Range(0.5f, 5f)]
    public float fadeSpeed = 2f;

    [Header("Audio")]
    public AudioClip completionReminderSound;
    private AudioSource audioSource;

    private bool hasShownMessage = false;

    void Start()
    {
        // Setup audio
        if (completionReminderSound != null)
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

        // Check puzzle completion state
        CheckPuzzleState();
    }

    private void CheckPuzzleState()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"[PuzzleStateManager] GameManager not found for puzzle: {puzzleID}");
            return;
        }

        bool isComplete = GameManager.Instance.IsPuzzleComplete(puzzleID);

        if (isComplete)
        {
            Debug.Log($"[PuzzleStateManager] Puzzle '{puzzleID}' already completed - showing completed state");
            
            // Set visual state
            SetCompletedState();

            // Show NPC message
            if (showCompletionMessage && !hasShownMessage)
            {
                StartCoroutine(ShowCompletionMessage());
            }
        }
        else
        {
            Debug.Log($"[PuzzleStateManager] Puzzle '{puzzleID}' not yet completed - showing incomplete state");
            SetIncompleteState();
        }
    }

    private void SetCompletedState()
    {
        // Show completed objects
        if (completedStateObjects != null)
        {
            foreach (GameObject obj in completedStateObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Hide incomplete objects
        if (incompleteStateObjects != null)
        {
            foreach (GameObject obj in incompleteStateObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    private void SetIncompleteState()
    {
        // Show incomplete objects
        if (incompleteStateObjects != null)
        {
            foreach (GameObject obj in incompleteStateObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }

        // Hide completed objects
        if (completedStateObjects != null)
        {
            foreach (GameObject obj in completedStateObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    private IEnumerator ShowCompletionMessage()
    {
        hasShownMessage = true;

        // Small delay before showing message
        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = alreadyCompletedMessage;
        }

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Play sound
        if (audioSource != null && completionReminderSound != null)
        {
            audioSource.PlayOneShot(completionReminderSound);
        }

        // Display message
        yield return new WaitForSeconds(messageDisplayDuration);

        // Fade out
        yield return StartCoroutine(FadeOut());
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

    [ContextMenu("Show Completion Message")]
    public void TestCompletionMessage()
    {
        hasShownMessage = false;
        StartCoroutine(ShowCompletionMessage());
    }
}