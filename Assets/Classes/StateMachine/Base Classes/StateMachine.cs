using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class StateMachine : ScriptableObject
{
    public List<State> states;

    private State current_state;
    private State previous_state;
    private bool initiated = false;


    private void OnDestroy()
    {
        foreach (State state in states)
        {
            if (state != null)
                Destroy(state);
        }
    }


    public void InitStateMachine(MonoBehaviour _behaviour)
    {
        for (int i = 0; i < states.Count; ++i)
        {
            if (states[i] != null)
            {
                states[i] = Object.Instantiate(states[i]);//make states unique instances
                states[i].InitState(_behaviour);
            }
        }

        current_state = states[0];
        initiated = true;
    }


    public void UpdateStateMachine()
    {
        if (!ReadyCheck())
            return;
        
        int state_id = current_state.ProcessTransitions();//check for transition trigger
        if (state_id != State.NO_TRANSITION)
        {
            AttemptTransition(state_id);
        }

        current_state.UpdateState();//before transitions?
    }


    void AttemptTransition(int _state_id)
    {
        current_state.OnStateExit();
        previous_state = current_state;//store last
        current_state = states.Find(x => x.state_id == _state_id);//switch to new state

        if (current_state == null)//failed transition
        {
            Debug.LogWarning("Transition Failed desired state not present. State id: " + _state_id);
            current_state = previous_state;
        }

        current_state.OnStateEnter();
    }


    bool ReadyCheck()
    {
        if (states.Count <= 0 || current_state == null)
            return false;

        if (!initiated)//only run if initiated
        {
            Debug.LogWarning("State machine not initiated!");
            return false;
        }

        return true;
    }

}
