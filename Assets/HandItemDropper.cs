using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HandItemDropper : MonoBehaviour
{
    public Transform hand_grip_transform;

    private GameObject held_game_object;
    private Rigidbody held_rigidbody;
    private Animator arm_animator;
    private bool dropping_item = false;


    void Start()
    {
        arm_animator = GetComponent<Animator>();
    }


    //called by animation event
    private void ReleaseObject()
    {
        dropping_item = false;

        if (held_game_object == null)
            return;

        held_game_object.transform.parent = null;//drop item

        if (held_rigidbody)
            held_rigidbody.isKinematic = false;//re-enable physics

        held_game_object = null;//clear held
        held_rigidbody = null;
    }


    public void PlaceObject(GameObject _game_object)
    {
        if (dropping_item)
        {
            Debug.Log("Hand Busy! Item Not Dropped");
            return;
        }
       
        AttachToHand(_game_object);
        dropping_item = true;
        arm_animator.SetTrigger("DropItem");
    }


    private void AttachToHand(GameObject _game_object)
    {
        held_game_object = _game_object;

        if (hand_grip_transform == null)
        {
            Debug.LogWarning("Hand Dropper Grip transform reference not set!");
            return;
        }

        held_game_object.transform.parent = hand_grip_transform;
        held_game_object.transform.localPosition = Vector3.zero;//centre on grip point
        DisableHeldObjectsPhysics();
    }


    private void DisableHeldObjectsPhysics()
    {
        held_rigidbody = held_game_object.GetComponent<Rigidbody>();
        if (held_rigidbody)
            held_rigidbody.isKinematic = true;//re-enable physics
    }

}
