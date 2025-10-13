// HelpPlayer state 
using UnityEngine;
using UnityEngine.AI;

public class HelpPlayerState : State {

    private NavMeshAgent agent;
    private GameObject player;
    private string puzzleName;

    public HelpPlayerState(GameObject npc, StateMachine sm, string puzzleName) : base(npc, sm) {
        this.puzzleName = puzzleName;
        agent = npc.GetComponent<NavMeshAgent>();
        player = Player.Instance.gameObject;
    }

    public override void Enter() {
        Debug.Log("Ghost is helping!");
    }

    public override void LogicUpdate() {
        Vector3 targetPos = player.transform.position + new Vector3(0, 2f, -2f);
        agent.SetDestination(targetPos);

        if (PuzzleManager.Instance.IsPuzzleComplete("Room 1")) {
            stateMachine.ChangeState(new VanishState(npc, stateMachine));
        }
    }
}
