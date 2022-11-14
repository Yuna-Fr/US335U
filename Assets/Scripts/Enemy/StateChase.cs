using UnityEngine;
using UnityEngine.AI;


public class StateChase : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.GetComponent<NavMeshAgent>().speed = 7;
        enemy.anim.Play("Run");
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (enemy.see == false /*|| enemy.IsBeingMoved == false*/)
            enemy.SwitchState(enemy.patrol);
    }
}
