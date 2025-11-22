using UnityEngine;

public class EngineRoomButton2Script : MonoBehaviour
{
    public Sprite unpushedButton;
    public Sprite pushedButton;

    public bool isPressed = false;

    public string Box = "Box";

    public GameObject Fire2;

    private SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Box))
        {
            PushButton();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Box))
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
        if (Fire2 != null)
            Fire2.SetActive(false);
    }

    void ReleaseButton()
    {
        if (!isPressed) return;

        isPressed = false;
        sr.sprite = unpushedButton;
    }
}
