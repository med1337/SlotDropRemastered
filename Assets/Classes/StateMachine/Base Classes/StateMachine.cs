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
        if (states.Count <= 0 || current_state == null)
            return;

        if (!initiated)
        {
            Debug.LogWarning("State machine not initiated!");
            return;
        }
        
        int state_id = current_state.ProcessTransitions();

        if (state_id != State.NO_TRANSITION)
        {
            current_state.OnStateExit();

            previous_state = current_state;
            current_state = states.Find(x => x.state_id == state_id);
            if (current_state == null)
            {
                Debug.LogWarning("Transition Failed desired state not present. State id: " + state_id);
                current_state = previous_state;
                return;
            }

            current_state.OnStateEnter();
        }

        current_state.UpdateState();//before transitions?
    }
}
