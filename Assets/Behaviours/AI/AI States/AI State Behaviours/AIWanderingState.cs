using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Wandering State")]
public class AIWanderingState : State
{
    private USBAI ai_controller;

    [Header("Transition to Attacking Params")]
    [Range(0, 100)]
    public float health_percent_attack_requirement = 30;

    [Space] [Header("Transition to Seek Slot Params")]
    public int required_score_to_slot = 50;
    public int panic_slot_health_requirement = 50;

    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int)AIState.Wandering;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller == null)
            return NO_TRANSITION;

        if (ai_controller.character.is_titan)
            return (int)AIState.Attacking;

        if (ai_controller.closest_enemy != null &&
            !ai_controller.CheckHealthPercentage(health_percent_attack_requirement))
            return (int)AIState.Attacking;

        bool slot_available = ai_controller.FindClosestOpenSlot();

        if (ai_controller.character.loadout_name == "Base" && slot_available)
            return (int)AIState.SeekSlot;

        if (ai_controller.CheckHealthPercentage(panic_slot_health_requirement) && slot_available)
            return (int)AIState.SeekSlot;

        if (ai_controller.character.stats.score >= required_score_to_slot && slot_available)
            return (int)AIState.SeekSlot;

        return NO_TRANSITION;
    }


    public override void UpdateState()
    {
        if (ai_controller == null)
            return;

        ai_controller.FindClosestEnemy();
        WanderBehaviour();
    }


    private void WanderBehaviour()
    {
        //TODO: Make AI wander to random points
    }

}
