using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private ButtonDetector button1;
    [SerializeField] private ButtonDetector button2;

    private bool doorOpened = false;

    // Reference to the door's components (e.g., Animator or SpriteRenderer)
    [SerializeField] private GameObject doorVisual;

    void Update()
    {
        if (!doorOpened && button1.IsCurrentlyPressed && button2.IsCurrentlyPressed)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log("Door opened!");

        if (doorVisual != null)
        {
            // Example: disable the door object to simulate opening
            doorVisual.SetActive(false);
        }

        // Optional: play sound, animation, etc.
    }
}
