using UnityEngine;

public class WaitingState : State {

    private float waitTime = 10f;
    private float timer;
    private string puzzleName;

    private SpriteRenderer laikaRenderer;

    public WaitingState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;

    }

    public override void Enter() {
        timer = 0f;
        Debug.Log("Ghost is waiting!");
        if(laikaRenderer != null) {
            laikaRenderer.enabled = false;
        }
    }

    public override void LogicUpdate() {
        timer += Time.deltaTime;

        if(timer >= waitTime) {
            Debug.Log("Ghost is appearing!");
            stateMachine.ChangeState(new AppearState(npc, stateMachine, puzzleName));
        }
    }

}