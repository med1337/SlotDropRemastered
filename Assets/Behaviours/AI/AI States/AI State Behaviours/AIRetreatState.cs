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


    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int)AIState.Retreating;
    }


    public override int ProcessTransitions()
    {
        if (ai_controller == null)
            return NO_TRANSITION;

        if (retreat_timer >= retreat_duration)
            return (int)AIState.Wandering;

        if (ai_controller.character.loadout_name == "Gold")
            return (int)AIState.Attacking;

            return NO_TRANSITION;
    }


    public override void UpdateState()
    {
        if (ai_controller == null)
            return;

        RetreatMovement();
        retreat_timer += Time.deltaTime;
    }


    void RetreatMovement()
    {
        if (ai_controller.closest_enemy == null)
            return;

        ai_controller.CalculateWaypoint();
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
