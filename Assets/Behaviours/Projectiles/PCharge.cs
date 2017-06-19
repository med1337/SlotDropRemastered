using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCharge : Projectile
{
    public float charge_speed;

    private Transform torso_mount;
    private Rigidbody rigid_body;
    private Vector3 absolute_direction;
    private float threshold = 0.5f;


	void Start()
    {
        if (owner)
        {
            torso_mount = owner.body_group.transform;
            rigid_body = owner.GetComponent<Rigidbody>();
        }

        TrackPlayer();
    }


    void Update()
    {
        CalculateRawDirection();
        TrackPlayer();
    }


    void FixedUpdate()
    {
        if (!owner)
            return;

        rigid_body.MovePosition(owner.transform.position + 
            (absolute_direction * Time.fixedDeltaTime * charge_speed));
    }


    void OnTriggerEnter(Collider other)
    {
        // Only collide with players.
        if (other.tag != "Player")
            return;

        USBCharacter character = other.GetComponent<USBCharacter>();

        // Don't collide with self.
        if (owner)
        {
            if (character == owner)
                return;
        }

        character.Damage(damage);
    }


    void CalculateRawDirection()
    {
        if (owner.last_facing.x > threshold)
        {
            absolute_direction = Vector3.right;
        }
        else if (owner.last_facing.x < -threshold)
        {
            absolute_direction = Vector3.left;
        }
        else if (owner.last_facing.z > threshold)
        {
            absolute_direction = Vector3.forward;
        }
        else if (owner.last_facing.z < -threshold)
        {
            absolute_direction = Vector3.back;
        }
    }

    
    void TrackPlayer()
    {
        if (torso_mount)
    	    transform.position = torso_mount.position;
    }

}
