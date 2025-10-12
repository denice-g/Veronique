using UnityEngine;
using System.Collections.Generic;

public class ButtonDetector : MonoBehaviour
{
    private bool hasBeenPressed = false;
    private HashSet<Collider2D> objectsOnButton = new HashSet<Collider2D>();

    public bool IsCurrentlyPressed => objectsOnButton.Count > 0;
    public bool HasBeenPressed => hasBeenPressed;

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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsOnButton.Remove(other);
        hasBeenPressed = objectsOnButton.Count > 0;
    }
}

