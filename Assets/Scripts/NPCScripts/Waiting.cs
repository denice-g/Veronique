using UnityEngine;

public class WaitingState : State {

    private float waitTime = 10f;
    private float timer = 0f;
    private string puzzleName;

    public WaitingState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;

    }

    public override void Enter() {
        timer = 0f;
        Debug.Log("Ghost is waiting!");
    }

    public override void LogicUpdate() {
        timer += Time.deltaTime;

        if(timer >= waitTime) {
            Debug.Log("Ghost is appearing!");
            stateMachine.ChangeState(new AppearState(npc, stateMachine, puzzleName));
        }
    }

}