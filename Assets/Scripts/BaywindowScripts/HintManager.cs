using UnityPro;
using TMPro;

public class HintManager : MonoBehaviour
{
    public enum HintState { Intro, Idle, Monitoring, Hinting, Victory }
    public HintState currentState = HintState.Intro;

    public CanvasGroup npcCanvas;
    public TMP_Text npcText;

    public int mistakesBeforeHint = 3;
    private int mistakeCount = 0;
    private WordPuzzleManager puzzleManager;

    void Start()
    {
        puzzleManager = FindObjectOfType<WordPUzzleManager>();
        FadeInNPC();
        ShowIntroText();
    }

    void Update()
    {
        switch (currentState)
        {
            case HintState.Intro:
                //after intro finishes, go to idle
                break;
            case HintState.Idle:
                //waiting for puzzle interactions
                break;
            case HintState.Monitoring:
                if (mistakeCount >= mistakesBeforeHint)
                {
                    EnterHintState();
                }
                break;
            case HintState.Hinting:
                //npc gives hint, then return to idle
                break;
            case HintState.Victory:
                //puzzle completed, npc congratulates and directs to next room
                break;
        }
    }

    public void RegisterIncorrectWord ()
    {
        mistakeCount++;
        currentState = HintState.Monitoring;
    }

    public void RegisterCorrectWord()
    {
        mistakeCount = 0;
    }

    public void RegisterPuzzleWin()
    {
        currentState = HintState.Victory;
        FadeInNPC();
        npcText.text = "YOU SOLVED THE PUZZLE! HEAD TO THE NEXT ROOM!";
    }

    private void ShowIntroText()
    {
        npcText.text = "WELCOME, DON'T THE STARS LOOK BEAUTIFUL! PRESS ON THE WINDOW TO GET A CLOSER LOOK.";
        currentState = HintState.Idle;
    }

    private void EnterHintState()
    {
        currentState = HintState.Hinting;
        FadeInNPC();

        string hint = puzzleManager.GetUnguessedWordHint();
        npcText.text = $"CAT HAVE LIMITED VOCABULARY, TRY THINKING OF A WORD LIKE.. MAYBER SOMETHING STARTING WITH '{hint}'?";

        mistakeCount = 0;
        currentState = HintState.Idle;
    }

    private void FadeInNPC()
    {
        npcCanvas.alpha = 1;
    }
    
    public void FadeOUtNPC()
    {
        npcCanvas.alpha = 0;
    }
}