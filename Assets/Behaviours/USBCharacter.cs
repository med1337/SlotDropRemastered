using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCharacter : MonoBehaviour
{
    public Projector shadow;
    public Rigidbody rigid_body;
    public bool face_locked;

    private USBLoadout loadout;
    private int health;

    private Vector3 move_dir;
    private Vector3 last_facing;


	void Start()
    {
		
	}
	

	void Update()
    {
		
	}


    void FixedUpdate()
    {
        rigid_body.MovePosition(transform.position + move_dir);
    }


    public void Move(Vector3 _dir)
    {
        _dir.Normalize();

        if (_dir != Vector3.zero)
        {
            if (!face_locked)
                last_facing = _dir;
        }

        move_dir = _dir * /* loadout.move_speed * */ Time.deltaTime;
    }


    public void Attack()
    {
        Debug.Log("Attack");
    }


    public void SlotDrop()
    {
        Debug.Log("SlotDrop");
    }


    public void AssignLoadout(USBLoadout _loadout)
    {
        // TODO: remove previous particle effect ..

        loadout = _loadout;
        health = _loadout.max_health;
        transform.localScale = _loadout.scale;
        shadow.orthographicSize = _loadout.scale.x;
    }

}
