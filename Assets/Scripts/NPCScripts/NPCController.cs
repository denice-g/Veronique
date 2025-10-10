// NPCController script
using UnityEngine;
using System.Collections; 

public class NPCController : MonoBehavior {
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