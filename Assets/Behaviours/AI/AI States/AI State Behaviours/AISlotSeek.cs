using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State Machine/AI State/Slot Seek State")]
public class AISlotSeek : State 
{
    private USBAI ai_controller;
    private bool slotted = false;

    public override void InitState(MonoBehaviour _behaviour)
    {
        ai_controller = _behaviour as USBAI;
        state_id = (int) AIState.SeekSlot;
    }

    public override int ProcessTransitions()
    {
        if (slotted)
            return (int) AIState.Wandering;


        return NO_TRANSITION;
    }

    public override void UpdateState()
    {
        ai_controller.FindClosestOpenSlot();
        slotted = MoveToSlot();
    }


    bool MoveToSlot()
    {
        if (ai_controller.closest_slot == null)
            return false;

        const float tolerance = 0.5f;

        ai_controller.MoveAI(ai_controller.CalculateMoveVector(ai_controller.closest_slot.transform), tolerance);

        if (Vector3.Distance(ai_controller.closest_slot.transform.position, ai_controller.character.transform.position) <= tolerance)
        {
            if (ai_controller.closest_slot.slottable)
            {
                ai_controller.character.SlotDrop();
                return true;
            }

            ai_controller.FindClosestOpenSlot();
            return false;
        }

        return false;
    }
}
