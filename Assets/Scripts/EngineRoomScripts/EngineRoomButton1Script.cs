using UnityEngine;

public class EngineRoomButton1Script : MonoBehaviour
{
    public Sprite unpushedButton;
    public Sprite pushedButton;

    public bool isPressed = false;

    public string Player = "Player";

    public GameObject Fire3;
    public GameObject Fire4;

    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            PushButton();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Player))
        {
            ReleaseButton();
        }
    }

    void PushButton()
    {
        if (isPressed) return;

        isPressed = true;
        sr.sprite = pushedButton;

        //Deactivate fire
        if (Fire3 != null && Fire4 != null)
            Fire3.SetActive(false);
            Fire4.SetActive(false);
    }

    void ReleaseButton()
    {
        if (!isPressed) return;

        isPressed = false;
        sr.sprite = unpushedButton;
    }
}
