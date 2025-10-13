// Trigger Initial NPC Interaction 
using UnityEngine;

public class NPCTrigger : MonoBehaviour {

    [SerializeField] private NPCController npcController;

    private bool hasAppeared = false;

    private void OnTriggerEnter(Collider other){
        if(!hasAppeared && other.CompareTag("Player")){
            hasAppeared = true;
            npcController.gameObject.SetActive(true);
        }
    }
}