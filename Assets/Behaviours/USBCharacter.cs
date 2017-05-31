using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCharacter : MonoBehaviour
{
    public Projector shadow;

    USBLoadout loadout;
    int health;


	void Start()
    {
		
	}
	

	void Update()
    {
		
	}


    public void Move(Vector3 _dir)
    {
        
    }


    public void Attack()
    {
        
    }


    public void SlotDrop()
    {

    }


    public void AssignLoadout(USBLoadout _loadout)
    {
        // TODO: remove previous particle effect ..

        loadout = _loadout;
        health = loadout.max_health;
        transform.localScale = loadout.scale;
        shadow.orthographicSize = loadout.scale.x;
    }

}
