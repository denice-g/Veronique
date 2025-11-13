using UnityEngine;
using UnityEngine.UI;
using System;

public class InteractCalander : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject calander;

    bool _playerInRange;
    bool _calandarOpen = false;

    void Start()
    {
        if (calander != null)
            calander.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = true;
            Console.Write("here");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
            Console.Write("here2");
            //if (calander) calander.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_playerInRange && !_calandarOpen)
            {
                calander.SetActive(true);
                _calandarOpen = true;
            }
            else if (_calandarOpen)
            {
                calander.SetActive(false);
                _calandarOpen = false;
            }
        }

    }
}
