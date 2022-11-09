using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class FSMState<StateInfo> where StateInfo : FSMStateInfo
{
    public string Name = "Undefined";
    public bool ShowDebug = false;
    public bool KeepMeAlive = false; //Si on doit me détruire à la fin de ma mise à jour

    public List<FSMState<StateInfo>> SubStates = new List<FSMState<StateInfo>>();
    public FSMState<StateInfo> ActiveSubState = null;
    private static int RecursionLevel = 0; //Pour afficher correctement la niveau hiérarchique de l'état lors du debug.

    public FSMState()
    {
        Name = GetType().ToString();
    }

    public void Update(ref StateInfo infos)
    {
        RecursionLevel++;

        log("Update");

        KeepMeAlive = false; //Je dois explicitement demander à rester en vie dans mon doState

        doState(ref infos);

        //Si on a pas de SubState actif, on dépile
        if (ActiveSubState == null)
        {
            if (SubStates.Count > 0)
                ActiveSubState = SubStates[SubStates.Count - 1]; //On prend le dernier à s'être activé
        }

        //Si on a un substate actif, on l'exécute
        if (ActiveSubState != null)
        {
            ActiveSubState.ShowDebug = this.ShowDebug;
            ActiveSubState.Update(ref infos);
            if (!ActiveSubState.KeepMeAlive)
            {
                removeSubState(ActiveSubState);
                log(ActiveSubState.Name + " has ended");
            }
        }

        //Si je n'ai pas demandé à rester en vie mais qu'un de mes enfants, actif ou non le veut, alors je dois rester en vie
        foreach (FSMState<StateInfo> state in SubStates)
            if (state.KeepMeAlive)
                KeepMeAlive = true;

        RecursionLevel--;
    }

    //Pour le comportement de l'état
    public abstract void doState(ref StateInfo infos);

    public bool isActiveSubstate<State>() where State : FSMState<StateInfo>
    {
        if (ActiveSubState != null && ActiveSubState.GetType() == typeof(State))
            return true;
        return false;
    }

    FSMState<StateInfo> findSubStateWithType<State>() where State : FSMState<StateInfo>
    {
        foreach (FSMState<StateInfo> state in SubStates)
            if (state.GetType() == typeof(State))
                return state;
        return null;
    }

    protected void addAndActivateSubState<State>() where State : FSMState<StateInfo>, new()
    {
        FSMState<StateInfo> state = findSubStateWithType<State>();
        if (state == null)
        {
            state = new State();
            log("Create " + state.Name);
        }
        else
        {
            SubStates.Remove(state);
        }

        //Ajouté en dernière position
        SubStates.Add(state);
        ActiveSubState = state;
    }

    protected void clearSubStates()
    {
        log("Cleared substates");
        SubStates.Clear();
        ActiveSubState = null;
    }

    protected void removeSubState<State>() where State : FSMState<StateInfo>, new()
    {
        FSMState<StateInfo> state = findSubStateWithType<State>();
        if (state != null)
        {
            log("Remove " + state.Name);
            SubStates.Remove(state);
        }
        if (ActiveSubState == state)
            ActiveSubState = null;
    }

    protected void removeSubState(FSMState<StateInfo> state)
    {
        if (SubStates.Remove(state))
        {
            log("Remove " + state.Name);
        }
    }

    protected void log(string message)
    {
        if (!ShowDebug)
            return;
        string msg = "";
        for (int i = 0; i < RecursionLevel; i++)
        {
            msg += "-";
        }
        Debug.Log(msg + " [" + Name + "] " + message);
    }
}

public class FSMachine<State, StateInfo>
    where State : FSMState<StateInfo>, new()
    where StateInfo : FSMStateInfo
{
    private State BaseState;
    public float PeriodUpdate = 0.3f;
    public bool ShowDebug;
    private float TempoUpdate = 0;

    public void Update(StateInfo infos)
    {
        if (BaseState == null)
        {
            BaseState = new State();
            return;
        }

        TempoUpdate += Time.deltaTime;
        if (TempoUpdate > PeriodUpdate)
        {
            TempoUpdate = 0;
            infos.PeriodUpdate = PeriodUpdate;
            BaseState.ShowDebug = ShowDebug;
            BaseState.Update(ref infos);
        }
    }
}

public class FSMStateInfo
{
    public float PeriodUpdate;
}

///                TST                /// 

[System.Serializable]
public class TSTStateInfo : FSMStateInfo
{
    public bool CanSeeTarget;
    public bool CloseToTarget;
    public float PcentLife;
}
public class TSTSBase : FSMState<TSTStateInfo>
{
    public float SeuilBonneSante = 1;
    public override void doState(ref TSTStateInfo infos)
    {
        if (infos.PcentLife >= SeuilBonneSante)
            addAndActivateSubState<TSTSBonneSante>();

        KeepMeAlive = true;
    }
}

public class TSTSBonneSante : FSMState<TSTStateInfo>
{
    public override void doState(ref TSTStateInfo infos)
    {
        if (infos.CanSeeTarget)
            addAndActivateSubState<TSTSAgressif>();
        else
            addAndActivateSubState<TSTSTranquille>();
    }
}

public class TSTSTranquille : FSMState<TSTStateInfo>
{
    public float PeriodIdle = 5;
    private float TempoIdle = 0;
    private bool Init = true;

    public override void doState(ref TSTStateInfo infos)
    {
        TempoIdle += infos.PeriodUpdate;

        if (TempoIdle > PeriodIdle || Init)
        {
            TempoIdle = 0;
            Init = false;
            if (isActiveSubstate<TSTSIdle>())
            {
                addAndActivateSubState<TSTSPatrouille>();
            }
            else
            {
                addAndActivateSubState<TSTSIdle>();
            }
        }

        KeepMeAlive = true; //Sinon on perds la tempo, l'init etc...
    }
}


public class TSTSAgressif : FSMState<TSTStateInfo>
{
    public override void doState(ref TSTStateInfo infos)
    {
        /*
        if (infos.CloseToTarget)
            addAndActivateSubState<TSTSAttaque>();
        else*/
            addAndActivateSubState<TSTSPoursuite>();
    }
}

public class TSTSIdle : FSMState<TSTStateInfo>
{
    public override void doState(ref TSTStateInfo infos)
    {
        Debug.Log("Je fais une petite anim d'idle");
    }
}


//=================================================================
                        // PATROLS //
public class TSTSPatrouille : FSMState<TSTStateInfo>
{
    public NavMeshAgent enemy;
    public Transform[] points;
    private int destPoint = 0;
    public override void doState(ref TSTStateInfo infos)
    {
        Debug.Log("Je fais ma petite patrouille...");

        //GotoNextPoint();
    }
    /*void GotoNextPoint()
    {
        
        if (points.Length == 0)// Returns if no points have been set up
            return;

        // Set the agent to go to the currently selected destination.
        enemy.destination = points[destPoint].position;

        // Choose the next point in the array as the destination, cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;

    }*/
}

public class TSTSPoursuite : FSMState<TSTStateInfo>
{
    public override void doState(ref TSTStateInfo infos)
    {
        Debug.Log("Je suis à sa poursuite !");
    }
}


public class TSTSFuite : FSMState<TSTStateInfo>
{
    public override void doState(ref TSTStateInfo infos)
    {
        Debug.Log("Je m'enfuie !");
    }
}