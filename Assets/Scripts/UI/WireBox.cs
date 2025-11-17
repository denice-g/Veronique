using UnityEngine;

public class Wirebox : MonoBehaviour
{
    private bool playerInRange = false;
    private Transform player;


    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!WireManager.Instance)
                return;

            // If no wire is active, start dragging one
            if (!WireManager.Instance.InstanceDragging)
            {
                WireManager.Instance.StartWire(this, player);
            }
            else
            {
                // Otherwise, try to connect to this box
                WireManager.Instance.TryConnectWire(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.transform;
            Debug.Log("Press E to interact with wirebox.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    // Optional callback for visual or logic updates
    public void OnConnected(Wirebox otherBox)
    {
        Debug.Log($"{name} connected to {otherBox.name}!");
        // You can add your puzzle logic here.
    }
}
