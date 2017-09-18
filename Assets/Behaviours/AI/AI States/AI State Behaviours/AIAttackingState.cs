using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Attacking State")]
public class AIAttackingState : State
{
    private USBAI ai_controller;

    [Header("Attack State Params")] public float max_distance_to_target = 5;

    [Space] [Header("Transition to Seek Params")] public int required_score_to_slot = 100;
    public int panic_slot_health_requirement = 10;

    [Space] [Header("Transition to Retreat Params")] public float low_health_retreat_chance = 0.5f;
    public int low_health_retreat_percentage = 30;
    public int always_retreat_health_percentage = 10;
    public float random_retreat_chance = 0.005f;



    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int) AIState.Attacking;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller == null)
            return NO_TRANSITION;

        if (ai_controller.closest_enemy == null)
            return (int) AIState.Wandering;

        int seek_result = CheckSlotSeekConditions();
        if (seek_result != NO_TRANSITION)
            return seek_result;

        int retreat_result = CheckRetreatConditions();
        if (retreat_result != NO_TRANSITION)
            return retreat_result;

        return NO_TRANSITION;
    }


    public override void UpdateState()
    {
        if (ai_controller == null)
            return;

        ChaseBehaviour();
        ai_controller.HandleBasic();
        ai_controller.HandleSpecial();
    }


    private void ChaseBehaviour()
    {
        if (ai_controller.closest_enemy == null || ai_controller.character == null)
            return;

        ai_controller.FindClosestEnemy();

        if (Vector3.Distance(ai_controller.closest_enemy.transform.position,
                ai_controller.character.transform.position) >
            max_distance_to_target)
            ai_controller.MoveAI(ai_controller.CalculateMoveVector(ai_controller.closest_enemy.transform),
                max_distance_to_target);
    }


    private int CheckRetreatConditions()
    {
        if (ai_controller.character.slot_dropping)
            return NO_TRANSITION;

        if (ai_controller.CheckHealthPercentage(always_retreat_health_percentage))
            return (int)AIState.Retreating;

        if (Random.Range(0, 100) < low_health_retreat_chance &&
            ai_controller.CheckHealthPercentage(low_health_retreat_percentage)) //roll for panic
            return (int)AIState.Retreating;

        if (Random.Range(0, 100) < random_retreat_chance)
            return (int)AIState.Retreating;

        return NO_TRANSITION;
    }


    private int CheckSlotSeekConditions()
    {
        bool available_slot = ai_controller.FindClosestOpenSlot();

        if (ai_controller.character.loadout_name == "Base" && available_slot)
            return (int)AIState.SeekSlot;

        if (ai_controller.character.stats.score >= required_score_to_slot && available_slot)
            return (int)AIState.SeekSlot;

        if (ai_controller.CheckHealthPercentage(panic_slot_health_requirement) && available_slot)
            return (int)AIState.SeekSlot;

        return NO_TRANSITION;
    }

}
