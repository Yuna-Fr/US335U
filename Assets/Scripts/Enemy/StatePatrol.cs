using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class StatePatrol : EnemyState
{
    private NavMeshAgent nav;
    private Transform enemyT;
    private Transform[] pts; //points où il passe pendant la patrouille 
    private int destPoint = 0;

    public override void EnterState(EnemyController enemy)
    {
        nav = enemy.GetComponent<NavMeshAgent>();
        pts = enemy.points;
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (pts.Length == 0)// Returns if no points have been set up
            return;

        // Set the agent to go to the currently selected destination.
        nav.destination = pts[destPoint].position;

        var distance = pts - enemyT;
        if (nav.destination <= 2) // if near change target
            destPoint = (destPoint + 1) % pts.Length; // Choose the next point in the array as the destination, cycling to the start if necessary.
        
        if (enemy.see) //if see player = chaseg
            enemy.SwitchState(enemy.chase);
    }
}
