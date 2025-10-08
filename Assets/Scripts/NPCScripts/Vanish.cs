// Vanish state 
using UnityEngine;

public class VanishState : State {

    private float fadeTime = 2f;
    private float timer = 0f;
    private Renderer ghostRenderer;

    public VanishState(GameObject npc, StateMachine sm) : base(npc, sm) {
        ghostRenderer = npc.GetComponent<Renderer>();
    }

    public override void Enter() {
        Debug.Log("Ghost is leaving!");
        timer = 0f;
    }

    public override void LogicUpdate() {
        timer += Time.deltaTime;

        if (timer > fadeTime) {
            npc.SetActive(false);
        }
    }
}