// NPCController script
using UnityEngine;

public class NPCController : MonoBehaviour {
    private StateMachine stateMachine;

    void OnEnable(){
        stateMachine = new StateMachine();
    }

    public void StartWaitingState() {
        var appear = new WaitingState(gameObject, stateMachine, "Room1");
        stateMachine.Initialize(appear);
    }

    void Update() {
        if (stateMachine.CurrentState != null) {
            stateMachine.CurrentState.LogicUpdate();
        }
    }
}