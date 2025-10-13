// NPCController script
using UnityEngine;

public class NPCController : MonoBehaviour {
    private StateMachine stateMachine;

    void OnEnable(){
        stateMachine = new StateMachine();
        var appear = new AppearState(gameObject, stateMachine);
        stateMachine.Initialize(appear);
    }

    void Update() {
        stateMachine.CurrentState.LogicUpdate();
    }
}