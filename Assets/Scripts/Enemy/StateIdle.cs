using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : EnemyState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.StartCoroutine("Lost");
    }

    public override void UpdateState(EnemyController enemy)
    {
            
    }
}
