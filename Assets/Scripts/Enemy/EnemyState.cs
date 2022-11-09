using UnityEngine;

public abstract class EnemyState
{
    public abstract void EnterState(EnemyController enemy);


    public abstract void UpdateState(EnemyController enemy);

}
