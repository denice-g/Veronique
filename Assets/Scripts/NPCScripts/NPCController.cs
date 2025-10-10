// NPCController script 

public class NPCController : Monobehavior {
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