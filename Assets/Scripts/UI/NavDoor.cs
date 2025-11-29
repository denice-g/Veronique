using UnityEngine;

public class NavDoor : MonoBehaviour
{
    [SerializeField] private WireBox wireBox1;
    [SerializeField] private WireBox wireBox2;
    [SerializeField] private WireBox wireBox3;
    [SerializeField] private GameObject doorVisual;
    private bool doorOpened = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!doorOpened && wireBox1.isConnected && wireBox2.isConnected && wireBox3.isConnected)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        doorOpened = true;
        Debug.Log("NavDoor opened!");

        if (doorVisual != null)
        {
            // rotate the door 60 degrees in Y
            // move the door to the right by 1 unit
            doorVisual.transform.Translate(1, 0, 0);
            doorVisual.transform.Rotate(0, 60, 0);
        }

        // Optional: play sound, animation, etc.
    }
}
