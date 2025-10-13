// Trigger Initial NPC Interaction 
using UnityEngine;

public class NPCTrigger : MonoBehaviour {

    public GameObject ghost;
    private bool hasAppeared = false;

    private void OnTriggerEnter(Collider other){
        if(!hasAppeared && other.CompareTag("Player")){
            hasAppeared = true;
            ghost.SetActive(true);
        }
    }
}