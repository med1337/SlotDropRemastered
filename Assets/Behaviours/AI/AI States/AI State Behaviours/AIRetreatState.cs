using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Retreat State")]
public class AIRetreatState : State 
{
    private USBAI ai_controller;
    private float retreat_timer = 0;

    [Header("Retreat State Params")]
    public float retreat_duration = 4f;

    [Space]
    [Header("Transition to Seek Slot Params")]
    public int panic_slot_health_percent = 10;

    [Space]
    [Header("Transition to Wandering Params")]
    public float enemy_proximity = 15;


    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int)AIState.Retreating;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller == null)
            return NO_TRANSITION;

        if (retreat_timer <= 0)
            return (int)AIState.Wandering;

        if (ai_controller.character.is_titan)
            return (int)AIState.Attacking;

        if (ai_controller.FindClosestOpenSlot() && ai_controller.CheckHealthPercentage(panic_slot_health_percent))
            return (int)AIState.SeekSlot;

        if (ai_controller.closest_enemy == null)
            return (int)AIState.Wandering;

        if (ai_controller.DistanceFromCharacter(ai_controller.closest_enemy.transform) < enemy_proximity)
            return (int)AIState.Wandering;

            return NO_TRANSITION;
    }


    public override void UpdateState()
    {
        if (ai_controller == null)
            return;

        RetreatMovement();
        retreat_timer -= Time.deltaTime;
    }


    void RetreatMovement()
    {
        if (ai_controller.closest_enemy == null)
            return;

        ai_controller.FindClosestEnemy();
        Vector3 dir = ai_controller.CalculateMoveVector(ai_controller.closest_enemy.transform);//invert vector
        dir *= -1;
        ai_controller.MoveAI(dir);//move in opposite direction
    }


    public override void OnStateEnter()
    {
        retreat_timer = retreat_duration;//reset panic
    }


    public override void OnStateExit()
    {
        retreat_timer = retreat_duration;//just to be sure
    }

}
