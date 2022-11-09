using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMTester : MonoBehaviour
{
    public TSTStateInfo FSMInfos = new TSTStateInfo();
    public bool ShowDebug = false;

    private FSMachine<TSTSBase, TSTStateInfo> FSM = new FSMachine<TSTSBase, TSTStateInfo>();

    void Update()
    {
        FSM.PeriodUpdate = 2;
        FSM.ShowDebug = ShowDebug;
        FSM.Update(FSMInfos);
    }
}

