using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public enum PlayerState
{
    WAITING,
    JOINING,
    PLAYING,
    LEAVING
}

public class ConnectedPlayer
{
    public Player input;
    public int id { get { return input.id; } }
    public PlayerState state = PlayerState.WAITING;
    public USBCharacter character = null;


    void HandleDropIn()
    {
		if (input.GetButtonDown("DropIn"))
        {
            if (state == PlayerState.WAITING)
            {
                state = PlayerState.JOINING;
            }
            else if (state == PlayerState.PLAYING)
            {
                state = PlayerState.LEAVING;
            }
        }
    }


    void ControlCharacter()
    {
        float horizontal = input.GetAxis("Horizontal");
        float vertical = input.GetAxis("Vertical");

        character.face_locked = input.GetButton("FaceLock");
        character.Move(new Vector3(horizontal, 0, vertical));

        if (input.GetButtonDown("Attack"))
        {
            character.Attack();
        }

        if (input.GetButtonDown("SlotDrop"))
        {
            character.SlotDrop();
        }
    }


    public void Update()
    {
        HandleDropIn();

        if (character)
        {
            ControlCharacter();
        }
    }


}
