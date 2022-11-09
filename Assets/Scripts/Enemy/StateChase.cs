using UnityEngine;

public class StateChase : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {

    }

    public override void UpdateState(EnemyController enemy)
    {
        if (enemy.see == false) //if see player = idle
            enemy.SwitchState(enemy.patrol);
    }
}
