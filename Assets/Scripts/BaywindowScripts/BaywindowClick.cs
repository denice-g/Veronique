using UnityEngine;

public class BaywindowClick : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject bigWindow;

    bool _playerInRange;

    bool _bigWindowOpen = false;

    void Start()
    {
        if (bigWindow != null)
        {
            bigWindow.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
        }
    }
    
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_playerInRange && !_bigWindowOpen)
            {
                bigWindow.SetActive(true);
                _bigWindowOpen = true;
            }
            else if (_bigWindowOpen)
            {
                bigWindow.SetActive(false);
                _bigWindowOpen = false;
            }
        }
    }
}