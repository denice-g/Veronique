using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Lever : MonoBehaviour
{
    [Header("Identity (must match line id)")]
    public string id = "ORANGE";          // ORANGE, YELLOW, GREEN, BLUE, RED

    [Header("Interaction")]
    public string playerTag = "Player";
    public KeyCode interactKey = KeyCode.E;

    [Header("Sprites")]
    public Sprite offSprite;
    public Sprite onSprite;

    [Header("Optional UI/SFX")]
    public GameObject promptUI;           // "Press E" icon (optional)
    public AudioSource sfx;
    public AudioClip flipOk;
    public AudioClip flipWrong;

    [HideInInspector] public LeverSequenceManager manager;

    SpriteRenderer sr;
    bool playerInside = false;
    bool isOn = false;
    public bool IsOn => isOn;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (promptUI) promptUI.SetActive(false);
        ApplyVisual();
    }

    void Update()
    {
        if (!playerInside) return;
        if (Input.GetKeyDown(interactKey))
        {
            manager?.AttemptFlip(this);
        }
    }

    public void SetOn(bool playSound = true)
    {
        isOn = true;
        ApplyVisual();
        if (playSound && flipOk) sfx?.PlayOneShot(flipOk);
    }

    public void SetOff(bool playSound = false)
    {
        isOn = false;
        ApplyVisual();
        if (playSound && flipWrong) sfx?.PlayOneShot(flipWrong);
    }

    void ApplyVisual()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!sr) return;

        sr.sprite = isOn && onSprite ? onSprite : offSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = true;
            if (promptUI) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            if (promptUI) promptUI.SetActive(false);
        }
    }
}
