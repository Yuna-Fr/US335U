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

    private float distance;

    public override void EnterState(EnemyController enemy)
    {
        enemy.GetComponent<NavMeshAgent>().speed = 3;
        nav = enemy.GetComponent<NavMeshAgent>();
        pts = enemy.points;
    }

    public override void UpdateState(EnemyController enemy)
    {
        if (pts.Length == 0)// Returns if no points have been set up
            return;

        nav.destination = pts[destPoint].position; // Set the agent to go to the currently selected destination.

        distance = Vector3.Distance(nav.destination, enemy.transform.position);
        if (distance <= 1) // if near/at destination, change target
            destPoint = (destPoint + 1) % pts.Length; // Choose the next point in the array as the destination, cycling to the start if necessary
        
        if (enemy.see) //if see player = chase
            enemy.SwitchState(enemy.chase);
    }
}
