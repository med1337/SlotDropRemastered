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
    public USBCharacter character;
    public Color color;


    public void Update()
    {
        HandleDropIn();

        if (character)
        {
            ControlCharacter();
        }
    }


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

        character.SetFaceLocked(input.GetButton("FaceLock"));
        character.Move(new Vector3(horizontal, 0, vertical));

        if (input.GetButton("Attack"))
        {
            character.Attack();
        }

        if (input.GetButtonDown("SlotDrop"))
        {
            character.SlotDrop();
        }
    }

}
