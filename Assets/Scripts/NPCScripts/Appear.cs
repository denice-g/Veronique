// Appear state 
using UnityEngine;

public class AppearState : State {

    private float appearDuration = 2f;
    private float timer = 0f;
    private Renderer ghostRenderer;
    private string puzzleName;

    public AppearState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;
        ghostRenderer = npc.GetComponent<Renderer>();
    }

    public override void Enter() {
        timer = 0f;
    }

    public override void LogicUpdate() {
        timer += Time.deltaTime;
        if(timer > appearDuration) {
            stateMachine.ChangeState(new HelpPlayerState(npc, stateMachine, puzzleName));
        }
    }
}
