// State code
using UnityEngine;

public abstract class State {

    protected GameObject npc;
    protected StateMachine stateMachine;

    protected State(GameObject npc, StateMachine stateMachine) {
        this.npc = npc;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void LogicUpdate() { }
    public virtual void Exit() { }
}