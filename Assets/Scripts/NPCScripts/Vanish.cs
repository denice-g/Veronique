// Vanish state 
using UnityEngine;

public class VanishState : State {

    //private float fadeTime = 2f;
    //private float timer = 0f;
    private Renderer laikaRenderer;
    private string puzzleName;

    public VanishState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;
        laikaRenderer = npc.GetComponent<Renderer>();
    }

    public override void Enter() {
        if (laikaRenderer != null) {
            laikaRenderer.enabled = false;
        }
        
        Debug.Log("Ghost is leaving!");
        //timer = 0f;
    }

    /*public override void LogicUpdate() {
        timer += Time.deltaTime;

        if (timer > fadeTime) {
            npc.SetActive(false);
        }
    }*/
}