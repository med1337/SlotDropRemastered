using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Attacking State")]
public class AIAttackingState : State 
{
    private USBAI ai_controller;

    public float max_distance_to_target = 5;
    public int required_score_to_cash = 100;
    public int panic_slot_health_requirement = 10;

    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int)AIState.Attacking;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller.closest_enemy == null)
            return (int)AIState.Wandering;

        if (ai_controller.character.stats.score >= required_score_to_cash ||
            ai_controller.CheckHealthPercentage(panic_slot_health_requirement))
            return (int)AIState.SeekSlot;

        if (ai_controller.RollForPanic())
            return (int)AIState.Retreating;

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

        ai_controller.CalculateWaypoint();

        if (Vector3.Distance(ai_controller.closest_enemy.transform.position, ai_controller.character.transform.position) >
            max_distance_to_target)
            ai_controller.MoveAI(ai_controller.CalculateMoveVector(ai_controller.closest_enemy.transform), max_distance_to_target);
    }

}
