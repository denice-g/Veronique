using UnityEngine;
using TMPro;
using System.Collections;

public class StorageHintManager : MonoBehaviour
{
    // ===== STATE MACHINE =====
    public enum StorageHintState
    {
        Intro,              // Welcome and direct to calendar
        WaitingCalendar,    // Waiting for player to examine calendar
        CalendarViewed,     // Player saw calendar, hint about roman numerals
        WaitingKey,         // Waiting for player to use key (after mistakes)
        KeyViewed,          // Player used key
        WaitingPassword,    // Monitoring password attempts
        Victory             // Success! Direct to next room
    }

    [Header("Current State")]
    public StorageHintState currentState = StorageHintState.Intro;

    [Header("UI Components")]
    public CanvasGroup npcCanvas;
    public TMP_Text npcText;
    public GameObject npcCharacter;

    [Header("Hint Settings")]
    [Tooltip("Number of wrong password attempts before showing key hint")]
    [Range(1, 10)]
    public int mistakesBeforeKeyHint = 3;

    [Header("Animation Settings")]
    [Range(0.5f, 5f)]
    public float fadeSpeed = 2f;

    [Range(2f, 10f)]
    public float messageDisplayDuration = 4f;

    public bool autoFadeAfterMessage = true;

    [Header("Dialogue Messages")]
    [TextArea(3, 5)]
    public string introMessage = "This room is a mess, woof. Looks like with all the commotion the computer disconnected. Somehow the calendar stayed on the wall... weird.";

    [TextArea(2, 4)]
    public string calendarReminderMessage = "Funny how the calendar is still on the wall...";

    [TextArea(2, 4)]
    public string calendarViewedMessage = "Interesting calendar... what are those weird letters. Might be important.";

    [TextArea(2, 4)]
    public string keyHintMessage = "I wonder what that blue note is, might be helpful.";

    [TextArea(2, 4)]
    public string keyViewedMessage = "Ah, it's a key! Now you can translate those letters from the calendar.";

    [TextArea(2, 4)]
    public string victoryMessage = "Excellent work! You cracked the code! The computer is back online - head through to the next area, woof.";

    [Header("Audio (Optional)")]
    public AudioClip hintAppearSound;
    public AudioClip victorySound;
    private AudioSource audioSource;

    private int passwordAttempts = 0;
    private bool isTransitioning = false;
    private bool hasViewedCalendar = false;
    private bool hasViewedKey = false;
    private Coroutine activeCoroutine;

    void Start()
    {
        // Validate UI references
        if (!ValidateUIReferences())
        {
            Debug.LogError("[StorageHintManager] Missing required UI references!");
            enabled = false;
            return;
        }

        // Setup audio
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
    }

    private bool ValidateUIReferences()
    {
        bool isValid = true;

        if (npcCanvas == null)
        {
            Debug.LogError("[StorageHintManager] NPC Canvas not assigned!");
            isValid = false;
        }

        if (npcText == null)
        {
            Debug.LogError("[StorageHintManager] NPC Text not assigned!");
            isValid = false;
        }

        return isValid;
    }

    void Update()
    {
        switch (currentState)
        {
            case StorageHintState.Intro:
                // Handled by coroutine
                break;

            case StorageHintState.WaitingCalendar:
                // Waiting for RegisterCalendarViewed() call
                break;

            case StorageHintState.CalendarViewed:
                // Handled by coroutine
                break;

            case StorageHintState.WaitingKey:
                // Waiting for RegisterKeyViewed() call
                break;

            case StorageHintState.KeyViewed:
                // Handled by coroutine
                break;

            case StorageHintState.WaitingPassword:
                // Check if player needs key hint
                if (passwordAttempts >= mistakesBeforeKeyHint && !hasViewedKey && !isTransitioning)
                {
                    TransitionToState(StorageHintState.WaitingKey);
                }
                break;

            case StorageHintState.Victory:
                // Handled by coroutine
                break;
        }
    }

    private void TransitionToState(StorageHintState newState)
    {
        Debug.Log($"[StorageHintManager] State: {currentState} → {newState}");
        currentState = newState;

        switch (newState)
        {
            case StorageHintState.WaitingKey:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = StartCoroutine(ShowKeyHint());
                break;

            case StorageHintState.Victory:
                if (activeCoroutine != null) StopCoroutine(activeCoroutine);
                activeCoroutine = StartCoroutine(VictorySequence());
                break;
        }
    }

    private IEnumerator IntroSequence()
    {
        currentState = StorageHintState.Intro;

        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = introMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);

        yield return new WaitForSeconds(messageDisplayDuration);

        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }

        TransitionToState(StorageHintState.WaitingCalendar);
    }

    private IEnumerator ShowKeyHint()
    {
        isTransitioning = true;

        if (npcText != null)
        {
            npcText.text = keyHintMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);

        yield return new WaitForSeconds(messageDisplayDuration);

        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }

        isTransitioning = false;
        // Stay in WaitingKey state until key is viewed
    }

    private IEnumerator VictorySequence()
    {
        currentState = StorageHintState.Victory;

        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = victoryMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(victorySound);

        Debug.Log("[StorageHintManager] Victory sequence displayed");
    }

    public void RegisterCalendarViewed()
    {
        if (hasViewedCalendar) return;

        hasViewedCalendar = true;
        Debug.Log("[StorageHintManager] Calendar viewed");

        if (currentState == StorageHintState.WaitingCalendar)
        {
            StartCoroutine(ShowCalendarMessage());
        }
    }

    private IEnumerator ShowCalendarMessage()
    {
        currentState = StorageHintState.CalendarViewed;

        yield return new WaitForSeconds(0.5f);

        if (npcText != null)
        {
            npcText.text = calendarViewedMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);

        yield return new WaitForSeconds(messageDisplayDuration);

        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }

        TransitionToState(StorageHintState.WaitingPassword);
    }

    public void RegisterKeyViewed()
    {
        if (hasViewedKey) return;

        hasViewedKey = true;
        Debug.Log("[StorageHintManager] Key viewed");

        if (currentState == StorageHintState.WaitingKey)
        {
            StartCoroutine(ShowKeyViewedMessage());
        }
    }

    private IEnumerator ShowKeyViewedMessage()
    {
        currentState = StorageHintState.KeyViewed;

        if (npcText != null)
        {
            npcText.text = keyViewedMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);

        yield return new WaitForSeconds(messageDisplayDuration);

        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }

        TransitionToState(StorageHintState.WaitingPassword);
    }

    public void RegisterPrematureComputerAttempt()
    {
        if (currentState == StorageHintState.WaitingCalendar && !hasViewedCalendar)
        {
            Debug.Log("[StorageHintManager] Player tried computer before calendar - redirecting");
            StartCoroutine(ShowCalendarReminder());
        }
    }

    private IEnumerator ShowCalendarReminder()
    {
        if (npcText != null)
        {
            npcText.text = calendarReminderMessage;
        }

        yield return StartCoroutine(FadeIn());
        PlaySound(hintAppearSound);

        yield return new WaitForSeconds(messageDisplayDuration);

        if (autoFadeAfterMessage)
        {
            yield return StartCoroutine(FadeOut());
        }
    }

    public void RegisterIncorrectPassword()
    {
        if (currentState == StorageHintState.Victory) return;

        passwordAttempts++;
        Debug.Log($"[StorageHintManager] Incorrect password attempt #{passwordAttempts}");

        if (currentState != StorageHintState.WaitingPassword)
        {
            currentState = StorageHintState.WaitingPassword;
        }
    }

    public void RegisterCorrectPassword()
    {
        Debug.Log("[StorageHintManager] Correct password!");
        TransitionToState(StorageHintState.Victory);
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

    public void ManualFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    public void ManualFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    public void ResetHintManager()
    {
        StopAllCoroutines();
        passwordAttempts = 0;
        hasViewedCalendar = false;
        hasViewedKey = false;
        isTransitioning = false;
        currentState = StorageHintState.Intro;

        if (npcCanvas != null)
        {
            npcCanvas.alpha = 0;
        }

        StartCoroutine(IntroSequence());
    }

    public string GetCurrentStateInfo()
    {
        return $"State: {currentState} | Password Attempts: {passwordAttempts}/{mistakesBeforeKeyHint} | Calendar: {hasViewedCalendar} | Key: {hasViewedKey}";
    }
}