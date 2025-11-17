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


    bool _playerInRange;
    bool _ScreenActive;
    string _currentInput = "";
    bool _puzzleCompleted = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (computerScreenUI != null) computerScreenUI.SetActive(false);

        if (puzzleCompleteLight != null) puzzleCompleteLight.SetActive(false);

        if (puzzleCompleteScreen != null) puzzleCompleteScreen.SetActive(false);

        if (puzzleCompleteScreenOff != null) puzzleCompleteScreenOff.SetActive(true);

        if (puzzleCompleteLightOff != null) puzzleCompleteLightOff.SetActive(true);

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

    // Update is called once per frame
    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(KeyCode.E) && !_puzzleCompleted)
        {
            _ScreenActive = !_ScreenActive;
            computerScreenUI.SetActive(_ScreenActive);

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

        }
        else
        {
            Debug.Log("incorrect");

        }

        ClearInput();
    }

   
}
