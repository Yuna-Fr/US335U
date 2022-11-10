using UnityEngine;
using UnityEngine.AI;


public class StateChase : EnemyState
{

    public override void EnterState(EnemyController enemy)
    {
        enemy.GetComponent<NavMeshAgent>().speed = 7;
    }

    public override void UpdateState(EnemyController enemy)
    {
        /*
        if (enemy.see == false) //if see player = idle
            enemy.SwitchState(enemy.patrol);
        */
    }
}
