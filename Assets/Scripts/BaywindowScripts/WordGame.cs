using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordGame : MonoBehaviour
{
    public TMP_Text currentWordText;
    public TMP_Text statusText;
    public TMP_Text wordsFoundText;

    public List<string> validWords;
    public int wordsNeededToWin = 10;

    private string currentWord = "";
    private int wordsFound = 0;
    private HashSet<string> wordsAlreadyFound = new HashSet<string>();

    public void AddLetter(string letter)
    {
        currentWord += letter;
        currentWordText.text = currentWord;
    }

    public void SubmitWord()
    {
        string word = currentWord.ToLower();

        if (validWords.Contains(word))
        {
            if (!wordsAlreadyFound.Contains(word))
            {
                wordsAlreadyFound.Add(word);
                wordsFound++;

                statusText.text = "CORRECT: " + word;
                wordsFoundText.text = "WORDS FOUND: " + wordsFound + " / " + wordsNeededToWin;

                if (wordsFound >= wordsNeededToWin)
                {
                    statusText.text = "YOU WIN!";
                }
            }
            else
            {
                statusText.text = "YOU ALREADY FOUND THIS WORD";
            }
        }
        else
        {
            statusText.text = "NOT A VALID WORD!";
        }

        currentWord = "";
        currentWordText.text = "";
    }

    public void ClearWord()
    {
        currentWord = "";
        currentWordText.text = "";
        statusText.text = "";
    }
}