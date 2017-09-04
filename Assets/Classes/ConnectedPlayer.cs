﻿using System.Collections;
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
#if UNITY_EDITOR
            DebugCheats();
#endif
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

        if (input.GetButtonDown("RB"))
        {
            character.score += 50;
        }
    }


    void DebugCheats()
    {
        if (!GameManager.cheats_enabled)
            return;

        if (input.GetButton("Back") && input.GetButton("Down") &&
            character != null)
        {
            DebugLoadoutSelect();
            DebugPlayerStats();
        }
    }


    void DebugLoadoutSelect()
    {
        if (input.GetButtonDown("SlotDrop"))
            LoadoutFactory.AssignLoadout(character, "Base");

        if (input.GetButtonDown("Y"))
            LoadoutFactory.AssignLoadout(character, "Fisher");

        if (input.GetButtonDown("B"))
            LoadoutFactory.AssignLoadout(character, "Trojan");

        if (input.GetButtonDown("Attack"))
            LoadoutFactory.AssignLoadout(character, "Pirate");
    }

    //put any stat related cheats here
    void DebugPlayerStats()
    {
        if (input.GetButtonDown("FaceLock"))
            character.Heal(99999);
    }
}
