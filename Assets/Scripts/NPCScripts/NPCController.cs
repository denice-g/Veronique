// NPCController script
using UnityEngine;

public class NPCController : MonoBehaviour {
    private StateMachine stateMachine;

    [SerializeField] private string puzzleName = "Room1";

    void OnEnable(){
        stateMachine = new StateMachine();

        var waiting = new WaitingState(gameObject, stateMachine, puzzleName);
        stateMachine.Initialize(waiting);
    }

    void Update() {
        //if (stateMachine.CurrentState != null) {
            stateMachine.CurrentState.LogicUpdate();
        //}
    }
}