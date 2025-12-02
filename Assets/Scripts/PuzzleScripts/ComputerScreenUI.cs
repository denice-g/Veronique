using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering;

public class ComputerScreenUI : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject computerScreenUI;
    public Text displayText;
    public string correctCode = "2025";

    public int maxDigits = 4;

    public GameObject puzzleCompleteLight;
    public GameObject puzzleCompleteScreen;

    public GameObject puzzleCompleteLightOff;
    public GameObject puzzleCompleteScreenOff;

    public GameObject SceneSwap;

    // ===== NEW: Add reference to puzzle manager =====
    [Header("Puzzle Integration")]
    public StoragePuzzleManager puzzleManager;

    bool _playerInRange;
    bool _ScreenActive;
    string _currentInput = "";
    bool _puzzleCompleted = false;

    void Start()
    {
        if (computerScreenUI != null) computerScreenUI.SetActive(false);

        if (puzzleCompleteLight != null) puzzleCompleteLight.SetActive(false);

        if (puzzleCompleteScreen != null) puzzleCompleteScreen.SetActive(false);

        if (puzzleCompleteScreenOff != null) puzzleCompleteScreenOff.SetActive(true);

        if (puzzleCompleteLightOff != null) puzzleCompleteLightOff.SetActive(true);

        if (puzzleManager == null)
        {
            puzzleManager = FindObjectOfType<StoragePuzzleManager>();
        }

        if (GameManager.Instance != null && 
        GameManager.Instance.IsPuzzleComplete("StoragePuzzle"))
        {
            _puzzleCompleted = true;
            
            // Set completed visual state
            if (puzzleCompleteLight != null) puzzleCompleteLight.SetActive(true);
            if (puzzleCompleteScreen != null) puzzleCompleteScreen.SetActive(true);
            if (puzzleCompleteScreenOff != null) puzzleCompleteScreenOff.SetActive(false);
            if (puzzleCompleteLightOff != null) puzzleCompleteLightOff.SetActive(false);
            if (SceneSwap != null) SceneSwap.SetActive(true);
            
            // Disable computer interaction
            computerScreenUI.SetActive(false);
            
            Debug.Log("[StoragePuzzle] Already complete - systems online");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
            _playerInRange = false;
    }

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !_puzzleCompleted)
        {
            _ScreenActive = !_ScreenActive;
            computerScreenUI.SetActive(_ScreenActive);

            // ===== NEW: Notify hint manager when computer opens =====
            if (_ScreenActive && puzzleManager != null)
            {
                // This will trigger redirect to calendar if player hasn't viewed it
                puzzleManager.OnPasswordAttempt(false); // Just to register computer access
            }

            if (!_ScreenActive)
                ClearInput();
        }
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
            displayText.text = _currentInput;
    }
    
    private void ClearInput()
    {
        _currentInput = "";
        UpdateDisplay();
    }

    public void OnNumberButtonPressed(string number)
    {
        if (!_ScreenActive || _puzzleCompleted) return;

        if (_currentInput.Length < maxDigits)
        {
            _currentInput += number;
            UpdateDisplay();
        }

        if (_currentInput.Length == maxDigits)
            CheckCode();
    }

    public void OnClearButtonPressed()
    {
        if (!_puzzleCompleted)
            ClearInput();
    }

    public void CheckCode()
    {
        if (_currentInput == correctCode)
        {
            Debug.Log("correct code");

            _puzzleCompleted = true;
            _ScreenActive = false;
            computerScreenUI.SetActive(false);

            if (puzzleCompleteLight != null) puzzleCompleteLight.SetActive(true);

            if (puzzleCompleteScreen != null) puzzleCompleteScreen.SetActive(true);

            if (puzzleCompleteScreenOff != null) puzzleCompleteScreenOff.SetActive(false);

            if (puzzleCompleteLightOff != null) puzzleCompleteLightOff.SetActive(false);

            if (SceneSwap != null) SceneSwap.SetActive(true);

            // ===== NEW: Notify puzzle manager of success =====
            if (puzzleManager != null)
            {
                puzzleManager.OnPasswordAttempt(true);
            }
        }
        else
        {
            Debug.Log("incorrect");

            // ===== NEW: Notify puzzle manager of failure =====
            if (puzzleManager != null)
            {
                puzzleManager.OnPasswordAttempt(false);
            }
        }

        ClearInput();
    }
}
