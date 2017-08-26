using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeBlock : MonoBehaviour 
{
    public ListenerModule listener_module = new ListenerModule();
    public float max_velocity = 10;
    public float snap_speed = 1;

    private bool move_to_slot = false;
    private Vector3 upgrade_slot_snap_position = Vector3.zero;
    private const float LERP_TOLERANCE = 0.5f;
    private float t = 0;
    private Rigidbody upgrade_cube_rigidbody;


    void Start()
    {
        upgrade_cube_rigidbody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        HandleUpgrade();
    }


    void FixedUpdate()
    {
        LimitVelocity();
    }


    void LimitVelocity()
    {
        if (upgrade_cube_rigidbody.velocity.magnitude > max_velocity)
            upgrade_cube_rigidbody.velocity = upgrade_cube_rigidbody.velocity.normalized * max_velocity;

    }


    void HandleUpgrade()
    {
        if (move_to_slot)
        {
            //check if move complete
            if (Vector3.Distance(transform.position, upgrade_slot_snap_position) <
                LERP_TOLERANCE)
            {
                move_to_slot = false;
                t = 0;
                upgrade_slot_snap_position = Vector3.zero;
                listener_module.NotifyListeners("UpgradedPC", this);//notify upgrade event
                return;
            }

            transform.position = Vector3.Lerp(transform.position,
                upgrade_slot_snap_position, t += Time.deltaTime * snap_speed);//lerp towards snap point
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "UpgradeSlot")
        {
            move_to_slot = true;
            upgrade_slot_snap_position = other.GetComponent<Collider>().bounds.center;//snap to centre of slot
        }
    }

}
