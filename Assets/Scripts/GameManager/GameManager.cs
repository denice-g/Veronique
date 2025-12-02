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
    public GameObject finalPuzzleDoor; // Door that unlocks when 4 puzzles complete

    [Header("Victory Settings")]
    public string creditsSceneName = "EndCredits";
    public float victoryDelay = 3f; // Delay before loading credits

    [Header("Debug")]
    public bool showDebugInfo = true;

    // Puzzle completion tracking
    private bool[] puzzlesCompleted;
    private bool finalPuzzleCompleted = false;
    private bool victoryTriggered = false;

    // PlayerPrefs keys
    private const string PUZZLE_PREFIX = "Puzzle_";
    private const string FINAL_PUZZLE_KEY = "FinalPuzzle_Completed";
    private const string GAME_COMPLETED_KEY = "Game_Completed";

    void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize puzzle tracking array
        puzzlesCompleted = new bool[puzzleNames.Length];

        // Load saved progress
        LoadProgress();

        Debug.Log("[GameManager] Initialized - Tracking " + puzzleNames.Length + " puzzles");
    }

    void Start()
    {
        // Check if all 4 puzzles are already complete
        if (AreAllMainPuzzlesComplete() && !finalPuzzleCompleted)
        {
            UnlockFinalPuzzle();
        }
    }

    public void CompletePuzzle(string puzzleName)
    {
        // Find puzzle index
        int index = System.Array.IndexOf(puzzleNames, puzzleName);

        if (index == -1)
        {
            Debug.LogWarning($"[GameManager] Unknown puzzle: {puzzleName}");
            return;
        }

        // Mark as complete
        if (!puzzlesCompleted[index])
        {
            puzzlesCompleted[index] = true;
            SaveProgress();

            Debug.Log($"[GameManager] Puzzle completed: {puzzleName} ({GetCompletedCount()}/{puzzleNames.Length})");

            // Check if all main puzzles are done
            if (AreAllMainPuzzlesComplete() && !finalPuzzleCompleted)
            {
                UnlockFinalPuzzle();
            }
        }
        else
        {
            Debug.Log($"[GameManager] Puzzle already completed: {puzzleName}");
        }
    }

    public void CompleteFinalPuzzle()
    {
        if (finalPuzzleCompleted)
        {
            Debug.Log("[GameManager] Final puzzle already completed");
            return;
        }

        finalPuzzleCompleted = true;
        PlayerPrefs.SetInt(FINAL_PUZZLE_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log("[GameManager] FINAL PUZZLE COMPLETED!");

        // Trigger victory sequence
        TriggerVictory();
    }

    public bool IsPuzzleComplete(string puzzleName)
    {
        int index = System.Array.IndexOf(puzzleNames, puzzleName);
        
        if (index == -1)
            return false;
        
        return puzzlesCompleted[index];
    }

    public bool AreAllMainPuzzlesComplete()
    {
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            if (!puzzlesCompleted[i])
                return false;
        }
        return true;
    }

    public int GetCompletedCount()
    {
        int count = 0;
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            if (puzzlesCompleted[i])
                count++;
        }
        return count;
    }


    private void UnlockFinalPuzzle()
    {
        Debug.Log("[GameManager] ✓ ALL 4 PUZZLES COMPLETE! Unlocking final puzzle...");

        // Unlock final puzzle door if assigned
        if (finalPuzzleDoor != null)
        {
            finalPuzzleDoor.SetActive(true);
            Debug.Log("[GameManager] Final puzzle door activated");
        }
    }

    private void TriggerVictory()
    {
        if (victoryTriggered)
            return;

        victoryTriggered = true;

        Debug.Log("[GameManager] 🎉 VICTORY! Starting end sequence...");

        // Mark game as completed
        PlayerPrefs.SetInt(GAME_COMPLETED_KEY, 1);
        PlayerPrefs.Save();

        // Start victory sequence
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        // Optional: Show victory UI, play sound, etc.
        Debug.Log("[GameManager] Victory delay...");

        yield return new WaitForSeconds(victoryDelay);

        // Load credits scene
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

        if (showDebugInfo)
        {
            Debug.Log($"[GameManager] Progress saved: {GetCompletedCount()}/{puzzleNames.Length} puzzles");
        }
    }

    private void LoadProgress()
    {
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            string key = PUZZLE_PREFIX + puzzleNames[i];
            puzzlesCompleted[i] = PlayerPrefs.GetInt(key, 0) == 1;
        }

        finalPuzzleCompleted = PlayerPrefs.GetInt(FINAL_PUZZLE_KEY, 0) == 1;

        if (showDebugInfo)
        {
            Debug.Log($"[GameManager] Progress loaded: {GetCompletedCount()}/{puzzleNames.Length} puzzles");
        }
    }

    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        // Clear puzzle completion
        for (int i = 0; i < puzzlesCompleted.Length; i++)
        {
            puzzlesCompleted[i] = false;
            string key = PUZZLE_PREFIX + puzzleNames[i];
            PlayerPrefs.DeleteKey(key);
        }

        // Clear final puzzle
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
        Debug.Log("[GameManager] DEBUG: All puzzles marked complete");
    }

    public string GetProgressString()
    {
        return $"{GetCompletedCount()}/{puzzleNames.Length} Puzzles Complete";
    }

    public string GetDetailedProgress()
    {
        string status = "=== PUZZLE PROGRESS ===\n";
        
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            string checkmark = puzzlesCompleted[i] ? "✓" : "✗";
            status += $"{checkmark} {puzzleNames[i]}\n";
        }

        status += $"\nFinal Puzzle: {(finalPuzzleCompleted ? "✓ Complete" : "✗ Locked")}\n";
        status += $"Victory: {(victoryTriggered ? "✓ Triggered" : "✗ Not triggered")}";

        return status;
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        // Debug display in top-left corner
        GUI.Box(new Rect(10, 10, 250, 150), "Game Manager");
        GUI.Label(new Rect(20, 35, 230, 20), GetProgressString());
        GUI.Label(new Rect(20, 55, 230, 20), $"Final: {(AreAllMainPuzzlesComplete() ? "Unlocked" : "Locked")}");
        
        int yPos = 75;
        for (int i = 0; i < puzzleNames.Length; i++)
        {
            string status = puzzlesCompleted[i] ? "✓" : "✗";
            GUI.Label(new Rect(20, yPos, 230, 20), $"{status} {puzzleNames[i]}");
            yPos += 18;
        }
    }
}