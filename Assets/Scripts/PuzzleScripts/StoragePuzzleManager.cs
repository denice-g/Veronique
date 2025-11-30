using UnityEngine;
using TMPro;

public class StoragePuzzleManager : MonoBehaviour
{
    [Header("Puzzle Elements")]
    [Tooltip("The calendar object that player clicks")]
    public GameObject calendarObject;

    [Tooltip("The Roman numeral key object")]
    public GameObject romanNumeralKey;

    [Header("Display Panels")]
    [Tooltip("Display panels/images when items are examined")]
    public GameObject calendarViewPanel;
    public GameObject keyViewPanel;

    [Header("Manager References")]
    public StorageHintManager hintManager;
    public VictoryArrow victoryArrow;
    public ComputerScreenUI computerScreen;

    private bool hasViewedCalendar = false;
    private bool hasViewedKey = false;
    private int incorrectAttempts = 0;

    void Start()
    {
        if (hintManager == null)
        {
            hintManager = FindObjectOfType<StorageHintManager>();

            if (hintManager == null)
            {
                Debug.LogWarning("[StoragPuzzleManager] StorageHintManager not found!");
            }
        }

        if (victoryArrow == null)
        {
            victoryArrow = FindObjectOfType<VictoryArrow>();
        }

        if (computerScreen == null)
        {
            computerScreen = FindObjectOfType<ComputerScreenUI>();

            if (computerScreen == null)
            {
                Debug.LogWarning("[StoragePuzzleManager] ComputerScreenUI not found!");
            }
        }

        HideAllPanels();
    }

    void Update()
    {
        if (computerScreen != null && computerScreen.computerScreenUI != null)
        {
            if (computerScreen.computerScreenUI.activeSelf && !hasViewedCalendar)
            {
                if (hintManager != null)
                {
                    hintManager.RegisterPrematureComputerAttempt();
                }
            }
        }
    }

    public void OnCalendarClicked()
    {
        Debug.Log("[StoragePuzzleManager] Calendar clicked");

        hasViewedCalendar = true;

        if (calendarViewPanel != null)
        {
            calendarViewPanel.SetActive(true);
        }

        if (hintManager != null)
        {
            hintManager.RegisterCalendarViewed();
        }
    }

    public void OnKeyClicked()
    {
        Debug.Log("[StoragePuzzleManager] Roman numeral key clicked");

        hasViewedKey = true;

        if (keyViewPanel != null)
        {
            keyViewPanel.SetActive(true);
        }

        if (hintManager != null)
        {
            hintManager.RegisterKeyViewed();
        }
    }

    public void OnPasswordAttempt(bool wasCorrect)
    {
        if (wasCorrect)
        {
            HandleCorrectPassword();
        }
        else
        {
            HandleIncorrectPassword();
        }
    }

    private void HandleCorrectPassword()
    {
        Debug.Log("[Storage Puzzle Manager] Puzzle completed!");

        if (hintManager != null)
        {
            hintManager.RegisterCorrectPassword();
        }

        if (victoryArrow != null)
        {
            victoryArrow.ShowVictoryArrow();
        }
    }

    private void HandleIncorrectPassword()
    {
        incorrectAttempts++;
        Debug.Log($"[StoragePuzzleManager] Incorrect password attempt #{incorrectAttempts}");

        if (hintManager != null)
        {
            hintManager.RegisterIncorrectPassword();
        }
    }

    public void CloseCalendarPanel()
    {
        if (calendarViewPanel != null)
        {
            calendarViewPanel.SetActive(false);
        }
    }

    public void CloseKeyPanel()
    {
        if ( keyViewPanel != null)
        {
            keyViewPanel.SetActive(false);
        }
    }

    private void HideAllPanels()
    {
        if (calendarViewPanel != null)
        {
            calendarViewPanel.SetActive(false);
        }

        if (keyViewPanel != null)
        {
            keyViewPanel.SetActive(false);
        }
    }

    public void ResetPuzzle()
    {
        hasViewedCalendar = false;
        hasViewedKey = false;
        incorrectAttempts = 0;

        HideAllPanels();

        if (victoryArrow != null)
        {
            victoryArrow.HideVictoryArrow();
        }

        if (hintManager != null)
        {
            hintManager.ResetHintManager();
        }

        Debug.Log("[StoragePuzzleManager] Puzzle reset");
    }
}
