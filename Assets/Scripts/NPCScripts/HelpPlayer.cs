// HelpPlayer state 
using UnityEngine;
using UnityEngine.AI;

public class HelpPlayerState : State {

    private NavMeshAgent agent;
    private GameObject player;

    public HelpPlayerState(GameObject npc, StateMachine sm) : base(npc, sm) {
        agent = npc.GetComponent<NavMeshAgent>();
        player = Player.Instance;
    }

    public override void Enter() {
        Debug.Log("Ghost is helping!");
    }

    public override void LogicUpdate() {
        Vector3 targetPos = player.transform.position + new Vector3(0, 2f, -2f);
        agent.SetDestination(targetPos);

        if (PuzzleManager.Instance.IsPuzzleComplete) {
            stateMachine.ChangeState(new VanishState(npc, stateMachine));
        }
    }
}
