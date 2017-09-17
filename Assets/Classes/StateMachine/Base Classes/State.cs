using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    [HideInInspector] public uint state_id = 0;
    public const int NO_TRANSITION = -99;

    public abstract void InitState(MonoBehaviour _behaviour);
    public virtual void OnStateEnter() {}
    public virtual void OnStateExit() {}
    /// <summary>
    /// Processes the states transitions.
    /// Should be made to return NO_TRANSITION if no transitions are triggered.
    ///  If a state transition does occur the new state should be returned as an int.
    /// </summary>
    public abstract int ProcessTransitions();
    public abstract void UpdateState();

}