// Appear state 
using UnityEngine;

public class AppearState : State {

    private float appearDuration = 2f;
    private float timer = 0f;
    private Renderer ghostRenderer;

    public AppearState(GameObject npc, StateMachine sm) : base(npc, sm) {
        ghostRenderer = npc.GetComponent<Renderer>();
    }

    public override void Enter() {
        timer = 0f;
    }

    public override void LogicUpdate() {
        timer += Timer.deltaTime;
        if(timer > appearDuration) {
            stateMachine.ChangeState(new HelpPlayerState(npc, stateMachine));
        }
    }
}
