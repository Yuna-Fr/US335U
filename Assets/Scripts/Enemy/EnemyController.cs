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

    private NavMeshAgent nav;

    //    ANIMATIONS    //
    public Animator anim;

    //    STATES    //
    public EnemyState currentState;
    public StateChase chase = new StateChase();
    public StatePatrol patrol = new StatePatrol();

    public Transform[] points;

    private Vector3 priorFrameTransform;
    //public Transform lens;

    public bool IsBeingMoved;


    void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        GetComponent<AISenseSight>().AddSenseHandler(new AISense<SightStimulus>.SenseEventHandler(HandleSight));
        GetComponent<AISenseSight>().AddObjectToTrack(player);
        GetComponent<AISenseHearing>().AddSenseHandler(new AISense<HearingStimulus>.SenseEventHandler(HandleHearing));
        GetComponent<AISenseHearing>().AddObjectToTrack(player);

        GetComponent<NavMeshAgent>();

        currentState = patrol;
        currentState.EnterState(this);

        IsBeingMoved = false;
        priorFrameTransform = transform.position;
    }

    private void Update()
    {
        currentState.UpdateState(this);

        if (Vector3.Distance(transform.position, priorFrameTransform) > 0.5f)
        {
            IsBeingMoved = true;
        }
        else
        {
                IsBeingMoved = false;
        }

        priorFrameTransform = transform.position;
    }

    public void SwitchState(EnemyState state)
    {
        currentState = state;
        state.EnterState(this);
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

