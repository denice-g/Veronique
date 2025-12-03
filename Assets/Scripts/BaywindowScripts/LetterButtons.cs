using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterButton : MonoBehaviour
{
    [Header("References")]
    public Button button;
    public TMP_Text buttonText;
    //public Image buttonImage;
    
    [Header("Visual States")]
    public Color normalColor = Color.white;
    public Color pressedColor = Color.gray;
    public float pressedAlpha = 0.5f;
    
    private bool isPressed = false;
    private string letter;
    private WordGame wordGame;
    
    void Awake()
    {
        // Auto-find components if not assigned
        if (button == null)
            button = GetComponent<Button>();
            
        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();
        
        // Get the letter from button text
        if (buttonText != null)
            letter = buttonText.text;
    }
    
    void Start()
    {
        // Find WordGame
        wordGame = FindObjectOfType<WordGame>();
        
        // Add click listener
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    void OnButtonClick()
    {
        // Only allow one press per word
        if (isPressed)
        {
            Debug.Log($"Button '{letter}' already pressed for this word!");
            return;
        }
        
        // Mark as pressed
        isPressed = true;
        
        // Update visual appearance
        SetPressedAppearance();
        
        // Send letter to WordGame
        if (wordGame != null)
        {
            wordGame.AddLetter(letter);
        }
    }
    
    private void SetPressedAppearance()
    {
        // Disable button interaction temporarily
        if (button != null)
        {
            button.interactable = false;
        }
        
        if (buttonText != null)
        {
            Color textColor = buttonText.color;
            textColor.a = pressedAlpha;
            buttonText.color = textColor;
        }
    }
  
    public void ResetButton()
    {

        Debug.Log("resetting now..");
        isPressed = false;
        
        // Re-enable button
        if (button != null)
        {
            Debug.Log("resetting now 2..");
            button.interactable = true;
        }
        
        if (buttonText != null)
        {
            Color textColor = buttonText.color;
            textColor.a = 1f;
            buttonText.color = textColor;
        }
    }

    public string GetLetter()
    {
        return letter;
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}