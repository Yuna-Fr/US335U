using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public bool ShowDebug = true;

    public bool see = false;
    public bool hear = false;

      //    STATES    //
    public EnemyState currentState;
    public StateChase chase = new StateChase();
    public StatePatrol patrol = new StatePatrol();

    public Transform[] points;

    void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<AISenseSight>().AddSenseHandler(new AISense<SightStimulus>.SenseEventHandler(HandleSight));
        GetComponent<AISenseSight>().AddObjectToTrack(player);
        GetComponent<AISenseHearing>().AddSenseHandler(new AISense<HearingStimulus>.SenseEventHandler(HandleHearing));
        GetComponent<AISenseHearing>().AddObjectToTrack(player);

        currentState = patrol;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(EnemyState state)
    {
        currentState = state;
        state.EnterState(this);
    }
    public IEnumerator Lost(EnemyController enemy)
    {
        yield return new WaitForSeconds(1.2f);
        enemy.SwitchState(enemy.patrol);
    }
    void HandleSight(SightStimulus sti, AISense<SightStimulus>.Status evt)
    {
        if (evt == AISense<SightStimulus>.Status.Enter)
        {
            Debug.Log("Objet " + evt + " vue en " + sti.position);

            see = true;
        }

        FindPathTo(sti.position);

        if ((sti.position - transform.position).sqrMagnitude < 2 * 2)
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    void HandleHearing(HearingStimulus sti, AISense<HearingStimulus>.Status evt)
    {
        if (evt == AISense<HearingStimulus>.Status.Enter)
        {
            Debug.Log("Objet " + evt + " ouïe en " + sti.position);
            hear = true;
        }
        FindPathTo(sti.position);
    }

    public void FindPathTo(Vector3 dest)
    {
        GetComponent<NavMeshAgent>().SetDestination(dest);
    }

    public void Stop()
    {
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    public void OnDrawGizmos()
    {
        if (!ShowDebug)
            return;

        float height = GetComponent<NavMeshAgent>().height;
        if (GetComponent<NavMeshAgent>().hasPath)
        {
            Vector3[] corners = GetComponent<NavMeshAgent>().path.corners;
            if (corners.Length >= 2)
            {
                Gizmos.color = Color.red;
                for (int i = 1; i < corners.Length; i++)
                {
                    Gizmos.DrawLine(corners[i - 1] + Vector3.up * height / 2, corners[i] + Vector3.up * height / 2);
                }
            }
        }
    }
    
}

