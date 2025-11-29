using UnityEngine;
using UnityEngine.UI;
using System;

public class InteractCalander : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject calander;

    public MonoBehaviour playerMoveScript;

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

                if (playerMoveScript != null)
                    playerMoveScript.enabled = false; //freezing the player


            }
            else if (_calandarOpen)
            {
                calander.SetActive(false);
                _calandarOpen = false;

                if (playerMoveScript != null)
                    playerMoveScript.enabled = true;

            }
        }

    }
}
