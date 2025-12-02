using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    [Header("Puzzle Tracking")]
    [Tooltip("Names of the 4 main puzzles")]
    public string[] puzzleNames = {
        "WordPuzzle",
        "LeverPuzzle", 
        "WirePuzzle",
        "StoragePuzzle"
    };

    [Header("Final Puzzle")]
    public string finalPuzzleName = "FinalPuzzle";
    public GameObject finalPuzzleDoor;

    [Header("Victory Settings")]
    public string creditsSceneName = "EndCredits";
    public float victoryDelay = 3f;

    [Header("Debug UI Settings")]
    public bool showDebugInfo = true;
    public KeyCode toggleUIKey = KeyCode.F1;
    public KeyCode hideUIKey = KeyCode.F2; // Completely hide/show UI
    
    [Header("UI Modes")]
    public UIDisplayMode displayMode = UIDisplayMode.Expanded;
    
    public enum UIDisplayMode
    {
        Expanded,      // Full window
        Minimized,     // Just title bar
        CompactCorner, // Small corner display
        Hidden         // Completely hidden
    }
    
    [Header("UI Appearance")]
    public Color headerColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
    public Color bodyColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color completeColor = Color.green;
    public Color incompleteColor = Color.red;
    
    [Header("UI Position")]
    public UICorner uiCorner = UICorner.TopLeft;
    
    public enum UICorner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Custom
    }
    
    public Vector2 customPosition = new Vector2(10, 10);
    public bool allowDragging = true;

    // Puzzle completion tracking
    private bool[] puzzlesCompleted;
    private bool finalPuzzleCompleted = false;
    private bool victoryTriggered = false;

    // PlayerPrefs keys
    private const string PUZZLE_PREFIX = "Puzzle_";
    private const string FINAL_PUZZLE_KEY = "FinalPuzzle_Completed";
    private const string GAME_COMPLETED_KEY = "Game_Completed";
    private const string UI_MODE_KEY = "UI_DisplayMode";

    // UI rects
    private Rect windowRect;
    private Vector2 dragOffset;
    
    // UI sizing
    private const float EXPANDED_WIDTH = 250f;
    private const float EXPANDED_HEIGHT = 160f;
    private const float MINIMIZED_WIDTH = 250f;
    private const float MINIMIZED_HEIGHT = 30f;
    private const float COMPACT_WIDTH = 120f;
    private const float COMPACT_HEIGHT = 40f;

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize puzzle tracking
        puzzlesCompleted = new bool[puzzleNames.Length];

        // Load saved progress
        LoadProgress();
        
        // Load UI display mode
        displayMode = (UIDisplayMode)PlayerPrefs.GetInt(UI_MODE_KEY, 0);
        
        // Initialize window position
        UpdateWindowPosition();

        Debug.Log("[GameManager] Initialized - Tracking " + puzzleNames.Length + " puzzles");
    }

    void Start()
    {
        if (AreAllMainPuzzlesComplete() && !finalPuzzleCompleted)
        {
            UnlockFinalPuzzle();
        }
    }
    
    void Update()
    {
        // Cycle through UI modes with F1
        if (Input.GetKeyDown(toggleUIKey))
        {
            CycleUIMode();
        }
        
        // Completely hide/show with F2
        if (Input.GetKeyDown(hideUIKey))
        {
            if (displayMode == UIDisplayMode.Hidden)
            {
                displayMode = UIDisplayMode.Expanded;
            }
            else
            {
                displayMode = UIDisplayMode.Hidden;
            }
            SaveUIMode();
        }
    }

    private void CycleUIMode()
    {
        // Cycle: Expanded → Minimized → Compact → Expanded
        switch (displayMode)
        {
            case UIDisplayMode.Expanded:
                displayMode = UIDisplayMode.Minimized;
                break;
            case UIDisplayMode.Minimized:
                displayMode = UIDisplayMode.CompactCorner;
                break;
            case UIDisplayMode.CompactCorner:
                displayMode = UIDisplayMode.Expanded;
                break;
            case UIDisplayMode.Hidden:
                displayMode = UIDisplayMode.Expanded;
                break;
        }
        
        SaveUIMode();
        UpdateWindowPosition();
        Debug.Log($"[GameManager] UI Mode: {displayMode}");
    }
    
    private void SaveUIMode()
    {
        PlayerPrefs.SetInt(UI_MODE_KEY, (int)displayMode);
        PlayerPrefs.Save();
    }
    
    private void UpdateWindowPosition()
    {
        float width = EXPANDED_WIDTH;
        float height = EXPANDED_HEIGHT;
        
        switch (displayMode)
        {
            case UIDisplayMode.Expanded:
                width = EXPANDED_WIDTH;
                height = EXPANDED_HEIGHT;
                break;
            case UIDisplayMode.Minimized:
                width = MINIMIZED_WIDTH;
                height = MINIMIZED_HEIGHT;
                break;
            case UIDisplayMode.CompactCorner:
                width = COMPACT_WIDTH;
                height = COMPACT_HEIGHT;
                break;
        }
        
        Vector2 position = customPosition;
        
        if (uiCorner != UICorner.Custom)
        {
            switch (uiCorner)
            {
                case UICorner.TopLeft:
                    position = new Vector2(10, 10);
                    break;
                case UICorner.TopRight:
                    position = new Vector2(Screen.width - width - 10, 10);
                    break;
                case UICorner.BottomLeft:
                    position = new Vector2(10, Screen.height - height - 10);
                    break;
                case UICorner.BottomRight:
                    position = new Vector2(Screen.width - width - 10, Screen.height - height - 10);
                    break;
            }
        }
        
        windowRect = new Rect(position.x, position.y, width, height);
    }

    // [Previous puzzle management methods remain the same]
    public void CompletePuzzle(string puzzleName)
    {
        int index = System.Array.IndexOf(puzzleNames, puzzleName);

        if (index == -1)
        {
            Debug.LogWarning($"[GameManager] Unknown puzzle: {puzzleName}");
            return;
        }

        if (!puzzlesCompleted[index])
        {
            puzzlesCompleted[index] = true;
            SaveProgress();

            Debug.Log($"[GameManager] Puzzle completed: {puzzleName} ({GetCompletedCount()}/{puzzleNames.Length})");

            if (AreAllMainPuzzlesComplete() && !finalPuzzleCompleted)
            {
                UnlockFinalPuzzle();
            }
        }
    }

    public void CompleteFinalPuzzle()
    {
        if (finalPuzzleCompleted) return;

        finalPuzzleCompleted = true;
        PlayerPrefs.SetInt(FINAL_PUZZLE_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log("[GameManager] FINAL PUZZLE COMPLETED!");
        TriggerVictory();
    }

    public bool IsPuzzleComplete(string puzzleName)
    {
        int index = System.Array.IndexOf(puzzleNames, puzzleName);
        return index != -1 && puzzlesCompleted[index];
    }

    public bool AreAllMainPuzzlesComplete()
    {
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            if (!puzzlesCompleted[i]) return false;
        }
        return true;
    }

    public int GetCompletedCount()
    {
        int count = 0;
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            if (puzzlesCompleted[i]) count++;
        }
        return count;
    }

    private void UnlockFinalPuzzle()
    {
        Debug.Log("[GameManager] ✓ ALL 4 PUZZLES COMPLETE! Unlocking final puzzle...");

        if (finalPuzzleDoor != null)
        {
            finalPuzzleDoor.SetActive(true);
            Debug.Log("[GameManager] Final puzzle door activated");
        }
    }

    private void TriggerVictory()
    {
        if (victoryTriggered) return;
        victoryTriggered = true;

        Debug.Log("[GameManager] 🎉 VICTORY! Starting end sequence...");
        PlayerPrefs.SetInt(GAME_COMPLETED_KEY, 1);
        PlayerPrefs.Save();

        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        Debug.Log("[GameManager] Victory delay...");
        yield return new WaitForSeconds(victoryDelay);

        Debug.Log($"[GameManager] Loading credits: {creditsSceneName}");
        SceneManager.LoadScene(creditsSceneName);
    }

    private void SaveProgress()
    {
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            string key = PUZZLE_PREFIX + puzzleNames[i];
            PlayerPrefs.SetInt(key, puzzlesCompleted[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            string key = PUZZLE_PREFIX + puzzleNames[i];
            puzzlesCompleted[i] = PlayerPrefs.GetInt(key, 0) == 1;
        }

        finalPuzzleCompleted = PlayerPrefs.GetInt(FINAL_PUZZLE_KEY, 0) == 1;
    }

    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            puzzlesCompleted[i] = false;
            string key = PUZZLE_PREFIX + puzzleNames[i];
            PlayerPrefs.DeleteKey(key);
        }

        finalPuzzleCompleted = false;
        victoryTriggered = false;
        PlayerPrefs.DeleteKey(FINAL_PUZZLE_KEY);
        PlayerPrefs.DeleteKey(GAME_COMPLETED_KEY);
        PlayerPrefs.Save();

        Debug.Log("[GameManager] All progress reset!");
    }

    [ContextMenu("Complete All Puzzles (Debug)")]
    public void DebugCompleteAll()
    {
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            CompletePuzzle(puzzleNames[i]);
        }
    }

    public string GetProgressString()
    {
        return $"{GetCompletedCount()}/{puzzleNames.Length}";
    }

    void OnGUI()
    {
        if (!showDebugInfo || displayMode == UIDisplayMode.Hidden) return;

        // Update position if corner-based
        if (uiCorner != UICorner.Custom)
        {
            UpdateWindowPosition();
        }

        // Create draggable window
        if (allowDragging)
        {
            windowRect = GUI.Window(0, windowRect, DrawDebugWindow, "");
        }
        else
        {
            DrawDebugWindow(0);
        }
    }
    
    void DrawDebugWindow(int windowID)
    {
        // Set colors
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = headerColor;
        
        // Draw based on mode
        switch (displayMode)
        {
            case UIDisplayMode.Expanded:
                DrawExpandedUI();
                break;
            case UIDisplayMode.Minimized:
                DrawMinimizedUI();
                break;
            case UIDisplayMode.CompactCorner:
                DrawCompactUI();
                break;
        }
        
        GUI.backgroundColor = originalColor;
        
        // Make draggable
        if (allowDragging)
        {
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
        }
    }
    
    void DrawExpandedUI()
    {
        // Header
        GUI.Box(new Rect(0, 0, windowRect.width, 25), "");
        GUI.Label(new Rect(10, 5, 150, 20), "Game Manager");
        
        // Mode cycle button
        if (GUI.Button(new Rect(windowRect.width - 30, 5, 20, 15), "−"))
        {
            CycleUIMode();
        }
        
        // Body background
        GUI.backgroundColor = bodyColor;
        GUI.Box(new Rect(0, 25, windowRect.width, windowRect.height - 25), "");
        
        // Progress
        GUI.Label(new Rect(10, 30, 230, 20), $"Progress: {GetProgressString()} Puzzles");
        
        // Final status
        Color statusColor = AreAllMainPuzzlesComplete() ? completeColor : incompleteColor;
        GUI.contentColor = statusColor;
        string finalStatus = AreAllMainPuzzlesComplete() ? "Unlocked" : "Locked";
        GUI.Label(new Rect(10, 50, 230, 20), $"Final: {finalStatus}");
        GUI.contentColor = Color.white;
        
        // Individual puzzles
        int yPos = 70;
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            Color puzzleColor = puzzlesCompleted[i] ? completeColor : incompleteColor;
            GUI.contentColor = puzzleColor;
            string status = puzzlesCompleted[i] ? "✓" : "✗";
            GUI.Label(new Rect(10, yPos, 230, 20), $"{status} {puzzleNames[i]}");
            yPos += 18;
            GUI.contentColor = Color.white;
        }
        
        // Controls hint
        GUI.Label(new Rect(10, windowRect.height - 20, 230, 20), $"[{toggleUIKey}] cycle | [{hideUIKey}] hide");
    }
    
    void DrawMinimizedUI()
    {
        // Minimized header only
        GUI.Box(new Rect(0, 0, windowRect.width, windowRect.height), "");
        
        // Title and expand button
        GUI.Label(new Rect(10, 8, 100, 20), "Game Manager");
        
        if (GUI.Button(new Rect(windowRect.width - 30, 8, 20, 15), "+"))
        {
            CycleUIMode();
        }
        
        // Compact progress
        Color progressColor = AreAllMainPuzzlesComplete() ? completeColor : incompleteColor;
        GUI.contentColor = progressColor;
        GUI.Label(new Rect(120, 8, 100, 20), GetProgressString());
        GUI.contentColor = Color.white;
    }
    
    void DrawCompactUI()
    {
        // Super compact corner display
        GUI.Box(new Rect(0, 0, windowRect.width, windowRect.height), "");
        
        // Just show progress with icon
        Color progressColor = AreAllMainPuzzlesComplete() ? completeColor : incompleteColor;
        GUI.contentColor = progressColor;
        
        string icon = AreAllMainPuzzlesComplete() ? "✓" : "○";
        GUI.Label(new Rect(10, 5, 100, 30), $"{icon} {GetProgressString()}");
        GUI.contentColor = Color.white;
        
        // Tiny expand button
        if (GUI.Button(new Rect(windowRect.width - 25, 5, 18, 30), "+"))
        {
            CycleUIMode();
        }
    }
}