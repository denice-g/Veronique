using UnityEngine;
using System.Collections.Generic;

public class ButtonDetector : MonoBehaviour
{
    private bool hasBeenPressed = false;
    private HashSet<Collider2D> objectsOnButton = new HashSet<Collider2D>();

    public bool IsCurrentlyPressed => objectsOnButton.Count > 0;
    public bool HasBeenPressed => hasBeenPressed;

    // On trigger - after 1 second change the sprite to pressed version, with X scale to 2
    private SpriteRenderer spriteRenderer;
    public Sprite unpressedSprite;
    public Sprite partiallyPressedSprite;
    // move partially pressed down by 0.5 units in Y axis


    

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && unpressedSprite != null)
        {
            spriteRenderer.sprite = unpressedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            objectsOnButton.Add(other);

            if (!hasBeenPressed)
            {
                hasBeenPressed = true;
                Debug.Log($"{name} was pressed!");
            }

            // after 0.5 seconds change sprite to partially pressed, after another 0.5 seconds change to fully pressed
            StartCoroutine(ChangeSpriteOverTime());

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsOnButton.Remove(other);
        hasBeenPressed = objectsOnButton.Count > 0;

        // If no objects are on the button, change sprite back to unpressed gradually
        if (objectsOnButton.Count == 0)
        {
            StartCoroutine(RevertSpriteOverTime());
        }
    }

    private System.Collections.IEnumerator ChangeSpriteOverTime()
    {
        if (spriteRenderer != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (IsCurrentlyPressed && partiallyPressedSprite != null)
            {
                spriteRenderer.sprite = partiallyPressedSprite;
            }
        }
    }

    private System.Collections.IEnumerator RevertSpriteOverTime()
    {
        if (spriteRenderer != null)
        {
            yield return new WaitForSeconds(0.5f);
            if (!IsCurrentlyPressed && unpressedSprite != null)
            {
                spriteRenderer.sprite = unpressedSprite;
            }
        }
    }

}

