using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Wandering State")]
public class AIWanderingState : State
{
    private USBAI ai_controller;
    [Range(0, 100)]
    public float health_percent_attack_requirement = 30;

    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int)AIState.Wandering;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller == null)
            return NO_TRANSITION;

        if (ai_controller.character.loadout_name == "Gold")
            return (int)AIState.Attacking;

        if (ai_controller.closest_enemy != null &&
            !ai_controller.CheckHealthPercentage(health_percent_attack_requirement))
            return (int)AIState.Attacking;

        if (ai_controller.RollForPanic())
            return (int)AIState.Retreating;

        if (ai_controller.character.stats.score >= 100)
            return (int)AIState.SeekSlot;

        return NO_TRANSITION;
    }


    public override void UpdateState()
    {
        if (ai_controller == null)
            return;

        ai_controller.CalculateWaypoint();
        WanderBehaviour();
    }


    private void WanderBehaviour()
    {
        //TODO: Make AI wander to random points
    }

}
