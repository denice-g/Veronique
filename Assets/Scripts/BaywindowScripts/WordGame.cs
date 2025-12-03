using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordGame : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text currentWordText;
    public TMP_Text statusText;
    public TMP_Text wordsFoundText;

    [Header("Game Objects")]
    public GameObject puzzlePanel;

    [Header("Game Settings")]
    public List<string> validWords = new List<string>();
    public int wordsNeededToWin = 10;

    [Header("Manager References")]
    public HintManager hintManager;
    public VictoryArrow victoryArrow;

    [Header("Letter Buttons")]
    public LetterButton[] letterButtons; // Drag all letter buttons here

    private string currentWord = "";
    private int wordsFound = 0;
    private HashSet<string> wordsAlreadyFound = new HashSet<string>();

    void Start()
    {
        // Auto-find HintManager if not assigned
        if (hintManager == null)
        {
            hintManager = FindObjectOfType<HintManager>();
            
            if (hintManager == null)
            {
                Debug.LogWarning("[WordGame] HintManager not found - hints disabled");
            }
        }

        // Auto-find VictoryArrow if not assigned
        if (victoryArrow == null)
        {
            victoryArrow = FindObjectOfType<VictoryArrow>();
        }

        // Auto-find letter buttons if not assigned
        if (letterButtons == null || letterButtons.Length == 0)
        {
            letterButtons = FindObjectsOfType<LetterButton>();
        }

        // Check if puzzle already complete
        if (GameManager.Instance != null && 
            GameManager.Instance.IsPuzzleComplete("WordPuzzle"))
        {
            // Puzzle already solved - disable puzzle panel
            if (puzzlePanel != null)
            {
                puzzlePanel.SetActive(false);
            }
            
            Debug.Log("[WordGame] Puzzle already complete - panel disabled");
            return; // Don't initialize puzzle
        }

        // Initialize UI
        UpdateWordsFoundUI();
        ClearWord();
    }
    
    public void AddLetter(string letter)
    {
        currentWord += letter;
        currentWordText.text = currentWord.ToUpper();
    }

    public void SubmitWord()
    {
        // Validate input
        if (string.IsNullOrEmpty(currentWord))
        {
            statusText.text = "ENTER A WORD FIRST!";
            return;
        }

        string word = currentWord.ToLower().Trim();

        // Check if word is in valid list
        if (validWords.Contains(word))
        {
            // Check if already found
            if (!wordsAlreadyFound.Contains(word))
            {
                HandleCorrectWord(word);
            }
            else
            {
                HandleDuplicateWord(word);
            }
        }
        else
        {
            HandleIncorrectWord(word);
        }

        // Clear the word input
        currentWord = "";
        if (currentWordText != null)
        {
            currentWordText.text = "";
        }
        
        // Reset all letter buttons so they can be pressed again
        ResetLetterButtons();
    }

    private void HandleCorrectWord(string word)
    {
        // Add to found words
        wordsAlreadyFound.Add(word);
        wordsFound++;

        // Update UI
        statusText.text = $"✓ CORRECT: {word.ToUpper()}";
        UpdateWordsFoundUI();

        // Notify hint manager
        if (hintManager != null)
        {
            hintManager.RegisterCorrectWord();
        }

        Debug.Log($"[WordGame] Correct word: {word} ({wordsFound}/{wordsNeededToWin})");

        // Check win condition
        if (wordsFound >= wordsNeededToWin)
        {
            WinPuzzle();
            GameManager.Instance?.CompletePuzzle("WordPuzzle");
        }
    }

    private void HandleIncorrectWord(string word)
    {
        statusText.text = $"✗ '{word.ToUpper()}' IS NOT VALID";

        // Notify hint manager of mistake
        if (hintManager != null)
        {
            hintManager.RegisterIncorrectWord();
        }

        Debug.Log($"[WordGame] Incorrect word: {word}");
    }

    private void HandleDuplicateWord(string word)
    {
        statusText.text = $"ALREADY FOUND: {word.ToUpper()}";
        Debug.Log($"[WordGame] Duplicate word: {word}");
    }

    private void WinPuzzle()
    {
        statusText.text = "PUZZLE COMPLETE!";

        // Notify hint manager of victory
        if (hintManager != null)
        {
            hintManager.RegisterPuzzleWin();
        }

        // Show victory arrow (no animation, just appears)
        if (victoryArrow != null)
        {
            victoryArrow.ShowVictoryArrow();
        }

        Debug.Log("[WordGame] Puzzle completed!");

        // Close puzzle panel after delay
        Invoke(nameof(ClosePuzzlePanel), 2f);
    }

    private void ClosePuzzlePanel()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
            Debug.Log("[WordGame] Puzzle panel closed");
        }
    }

    public void ClearWord()
    {
        currentWord = "";
        if (currentWordText != null)
        {
            currentWordText.text = "";
        }
        
        // Reset letter buttons so they can be pressed again
        ResetLetterButtons();
    }

    public void ResetLetterButtons()
    {
        if (letterButtons != null)
        {
            Debug.Log("[WordGame] Resetting Buttons... ");
            foreach (LetterButton button in letterButtons)
            {
                if (button != null)
                {
                    button.ResetButton();
                }
            }
        }
    }

    private void UpdateWordsFoundUI()
    {
        if (wordsFoundText != null)
        {
            wordsFoundText.text = $"WORDS: {wordsFound} / {wordsNeededToWin}";
        }
    }

    public string GetUnguessedWordHint()
    {
        foreach (string word in validWords)
        {
            if (!wordsAlreadyFound.Contains(word))
            {
                return word.Substring(0, 1).ToUpper();
            }
        }

        return "?";
    }

    public string GetRandomUnguessedWordHint()
    {
        List<string> unguessed = new List<string>();
        
        foreach (string word in validWords)
        {
            if (!wordsAlreadyFound.Contains(word))
            {
                unguessed.Add(word);
            }
        }

        if (unguessed.Count > 0)
        {
            return unguessed[Random.Range(0, unguessed.Count)].Substring(0, 1).ToUpper();
        }

        return "?";
    }

    public void ResetPuzzle()
    {
        currentWord = "";
        wordsFound = 0;
        wordsAlreadyFound.Clear();
        
        ClearWord();
        statusText.text = "";
        UpdateWordsFoundUI();

        // Reset letter buttons
        ResetLetterButtons();

        // Hide victory arrow
        if (victoryArrow != null)
        {
            victoryArrow.HideVictoryArrow();
        }

        if (hintManager != null)
        {
            hintManager.ResetHintManager();
        }

        Debug.Log("[WordGame] Puzzle reset");
    }
}