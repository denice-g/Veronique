using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShipCrisisController : MonoBehaviour
{
    public static ShipCrisisController Instance { get; private set; }

    [Header("Timer")]
    [Tooltip("Seconds the player has to fix the ship.")]
    public float crisisDuration = 60f;

    [Tooltip("Text element at the top of the screen to display the countdown.")]
    public TextMeshProUGUI timerLabel;

    [Tooltip("Optional: CanvasGroup to fade timer UI in/out.")]
    public CanvasGroup timerCanvasGroup;

    [Header("Game Over / UI")]
    [Tooltip("Panel to show when time runs out (End Screen).")]
    public GameObject endScreenPanel;

    [Tooltip("Stop time when game ends.")]
    public bool pauseOnEnd = true;

    public bool InCrisis { get; private set; }
    public float TimeLeft { get; private set; }

    private AudioManager audioManager;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
         DontDestroyOnLoad(gameObject); // Uncomment if you want this to persist across scenes
        HideTimerUI();
        if (endScreenPanel) endScreenPanel.SetActive(false);
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (!InCrisis) return;

        TimeLeft -= Time.deltaTime;
        UpdateTimerLabel(TimeLeft);

        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            EndGame();
        }
    }

    public void TriggerCrisis()
    {
        if (InCrisis) return; // already running
        InCrisis = true;
        TimeLeft = crisisDuration;
        ShowTimerUI();
        UpdateTimerLabel(TimeLeft);

        audioManager.PlayMusic(audioManager.gameStartMusic);

        // TODO: Trigger VFX/SFX/alarms/ship “broken” state here
    }

    public void FixShip()
    {
        if (!InCrisis) return;
        InCrisis = false;
        HideTimerUI();
        // TODO: Revert ship visuals/SFX back to normal here
    }

    void EndGame()
    {
        InCrisis = false;
        HideTimerUI();

        if (endScreenPanel) endScreenPanel.SetActive(true);

        if (pauseOnEnd) Time.timeScale = 0f;

        // If you prefer a separate “Game Over” scene, replace above with:
        // SceneManager.LoadScene("GameOverSceneName");
    }

    void UpdateTimerLabel(float seconds)
    {
        if (!timerLabel) return;
        int s = Mathf.Max(0, Mathf.CeilToInt(seconds));
        int m = s / 60;
        int sec = s % 60;
        timerLabel.text = $"{m:0}:{sec:00}";
    }

    void ShowTimerUI()
    {
        if (timerCanvasGroup)
        {
            timerCanvasGroup.alpha = 1f;
            timerCanvasGroup.interactable = false;
            timerCanvasGroup.blocksRaycasts = false;
        }
        else if (timerLabel)
        {
            timerLabel.gameObject.SetActive(true);
        }
    }

    void HideTimerUI()
    {
        if (timerCanvasGroup)
        {
            timerCanvasGroup.alpha = 0f;
            timerCanvasGroup.interactable = false;
            timerCanvasGroup.blocksRaycasts = false;
        }
        else if (timerLabel)
        {
            timerLabel.gameObject.SetActive(false);
        }
    }
}
