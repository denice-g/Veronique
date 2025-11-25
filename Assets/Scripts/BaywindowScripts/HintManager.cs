using UnityEngine;
using TMPro;
using System.Collections;

public class HintManager : MonoBehaviour
{
    public enum HintState 
    {  Intro,          // Initial welcome and instructions
        Idle,           // Silent monitoring
        Monitoring,     // Tracking mistakes
        Hinting,        // Providing assistance
        Victory         // Celebration and direction
    }
    
    [Header("Current State")]
    public HintState currentState = HintState.Intro;

    [Header("UI Components")]
    [Tooltip("Canvas Group component for fade effects")]
    public CanvasGroup npcCanvas;
    
    [Tooltip("TextMeshPro component for NPC dialogue")]
    public TMP_Text npcText;
    
    [Tooltip("Optional: NPC character image/sprite")]
    public GameObject npcCharacter;

    [Header("Hint Settings")]
    [Tooltip("Number of mistakes before hint appears")]
    [Range(1, 10)]
    public int mistakesBeforeHint = 3;
    
    [Header("Animation Settings")]
    [Tooltip("Speed of fade in/out animations")]
    [Range(0.5f, 5f)]
    public float fadeSpeed = 5f;
    
    [Tooltip("How long messages stay on screen")]
    [Range(2f, 10f)]
    public float messageDisplayDuration = 10f;
    
    [Tooltip("Auto-hide NPC after displaying message")]
    public bool autoFadeAfterMessage = true;
    
    [TextArea(3, 5)]
    public string introMessage = "Welcome! The stars are beautiful tonight.\nClick on the window to start the puzzle.";
    
    [Range(3f, 15f)]
    public float introDisplayTime = 5f;
    
    [TextArea(2, 4)]
    public string hintMessageTemplate = "Having trouble? Try thinking of a word starting with '{0}'...";
    
    [TextArea(2, 4)]
    public string victoryMessage = "Incredible! You solved it!\nHead through the door to the next puzzle room.";
    
    [TextArea(2, 4)]
    public string encouragementMessage = "You're doing great! Keep trying!";

    public AudioClip hintAppearSound;
    public AudioClip victorySound;
    private AudioSource audioSource;

    private int mistakeCount = 0;
    private WordGame wordGame;
    private bool isTransitioning = false;
    private Coroutine activeCoroutine;

    void Start()
    {
        // Find or assign WordGame reference
        if (wordGame == null)
        {
            wordGame = FindObjectOfType<WordGame>();
            if (wordGame == null)
            {
                Debug.LogError("[HintManager] WordGame not found! System disabled.");
                enabled = false;
                return;
            }
        }
        
        // Setup audio if sounds are assigned
        if (hintAppearSound != null || victorySound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Initialize UI
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
            npcCanvas.interactable = false;
            npcCanvas.blocksRaycasts = false;
        }
        
        // Start intro sequence
        StartCoroutine(IntroSequence());
        
        Debug.Log("[HintManager] Initialized - State: Intro");
    } 

    void Update()
    {
        switch (currentState)
        {
            case HintState.Intro:
                // Handled by IntroSequence coroutine
                break;
                
            case HintState.Idle:
                // Silent monitoring, no action needed
                break;
                
            case HintState.Monitoring:
                // Check if hint threshold reached
                if (mistakeCount >= mistakesBeforeHint && !isTransitioning)
                {
                    TransitionToState(HintState.Hinting);
                }
                break;
                
            case HintState.Hinting:
                // Handled by ProvideHint coroutine
                break;
                
            case HintState.Victory:
                // Handled by VictorySequence coroutine
                break;
        }
    }

    private void TransitionToState(HintState newState)
    {
        Debug.Log($"[HintManager] State: {currentState} → {newState}");
        currentState = newState;
        
        switch (newState)
        {
            case HintState.Hinting:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = StartCoroutine(ProvideHint());
                break;
                
            case HintState.Victory:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = StartCoroutine(VictorySequence());
                break;
        }
    }
 
    private IEnumerator IntroSequence()
    {
        currentState = HintState.Intro;
        
        // Small delay before NPC appears
        yield return new WaitForSeconds(0.5f);
        
        // Display intro message
        npcText.text = introMessage;
        yield return StartCoroutine(FadeIn());
        
        // Play sound if available
        PlaySound(hintAppearSound);
        
        // Hold message on screen
        yield return new WaitForSeconds(introDisplayTime);
        
        // Fade out
        yield return StartCoroutine(FadeOut());
        
        // Transition to idle
        TransitionToState(HintState.Idle);
        
        Debug.Log("[HintManager] Intro complete, entering Idle state");
    }

    private IEnumerator ProvideHint()
    {
        currentState = HintState.Hinting;
        isTransitioning = true;
        
        // Get hint letter from unguessed words
        string hintLetter = wordGame.GetUnguessedWordHint();
        
        // Format hint message
        string hintMessage = string.Format(hintMessageTemplate, hintLetter.ToUpper());
        npcText.text = hintMessage;
        
        // Fade in with hint
        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);
        
        Debug.Log($"[HintManager] Hint provided: {hintLetter}");
        
        // Display hint
        yield return new WaitForSeconds(messageDisplayDuration);
        
        // Fade out if auto-fade enabled
        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }
        
        // Reset mistake counter and return to idle
        mistakeCount = 0;
        isTransitioning = false;
        TransitionToState(HintState.Idle);
    }

    private IEnumerator VictorySequence()
    {
        currentState = HintState.Victory;
        
        // Brief pause for puzzle completion animation
        yield return new WaitForSeconds(1f);
        
        // Display victory message
        npcText.text = victoryMessage;
        yield return StartCoroutine(FadeIn());
        
        // Play victory sound
        PlaySound(victorySound);
        
        Debug.Log("[HintManager] Victory sequence displayed");
        
        // Keep victory message visible
        // (Will stay until player moves to next room)
    }

    public void RegisterIncorrectWord()
    {
        if (currentState == HintState.Victory) return;
        
        mistakeCount++;
        Debug.Log($"[HintManager] Mistake #{mistakeCount} registered");
        
        // Transition from Idle to Monitoring
        if (currentState == HintState.Idle)
        {
            TransitionToState(HintState.Monitoring);
        }
    }
    
    public void RegisterCorrectWord()
    {
        if (currentState == HintState.Victory) return;
        
        // Reset mistakes on success
        mistakeCount = 0;
        Debug.Log("[HintManager] Correct word - mistakes reset");
        
        // Return to Idle if monitoring
        if (currentState == HintState.Monitoring)
        {
            TransitionToState(HintState.Idle);
        }
    }
    
    public void RegisterPuzzleWin()
    {
        if (currentState == HintState.Victory) return;
        
        Debug.Log("[HintManager] Puzzle complete - triggering victory");
        TransitionToState(HintState.Victory);
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
 
    public void HideNPC()
    {
        StartCoroutine(FadeOut());
    }

    public void ShowNPC()
    {
        StartCoroutine(FadeIn());
    }

    public void ResetHintManager()
    {
        StopAllCoroutines();
        mistakeCount = 0;
        isTransitioning = false;
        currentState = HintState.Intro;
        
        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
        }
        
        StartCoroutine(IntroSequence());
    }

    public string GetCurrentStateInfo()
    {
        return $"State: {currentState} | Mistakes: {mistakeCount}/{mistakesBeforeHint}";
    }
}