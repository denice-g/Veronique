// Appear state 
using UnityEngine;

public class AppearState : State {

    private float timer;
    private Renderer laikaRenderer;
    private string puzzleName;
    private bool hasSpoken = false;

    public AppearState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;
        laikaRenderer = npc.GetComponent<Renderer>();
    }

    public override void Enter() {

        if (laikaRenderer != null) {
            laikaRenderer.enabled = true;
        }

        Debug.Log("Laika appeared!");
        timer = 0f;

        if (!hasSpoken) {
            DialogManager.Instance.ShowDialog("You seem lost... try moving the box woof.", 5f);
            hasSpoken = true;
        }
    }

    public override void LogicUpdate() {
        timer += Time.deltaTime;
        if(PuzzleManager.Instance.IsPuzzleComplete(puzzleName)) {
            stateMachine.ChangeState(new VanishState(npc, stateMachine, puzzleName));
        }
    }
}
