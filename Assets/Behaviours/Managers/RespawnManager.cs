﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] GameObject usb_character_prefab;

    private static RespawnManager instance;


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;
    }


    void Update()
    {
        RespawnPlayers();
    }


    void RespawnPlayers()
    {
        foreach (ConnectedPlayer player in PlayerManager.players)
        {
            if (PlayerAwaitingRespawn(player))
            {
                RespawnPlayer(player);
            }
        }
    }


    bool PlayerAwaitingRespawn(ConnectedPlayer _player)
    {
        return _player.state == PlayerState.PLAYING && _player.character == null;
    }


    void RespawnPlayer(ConnectedPlayer _player)
    {
        // Create and position the USBCharacter.
        _player.character = Instantiate(usb_character_prefab).GetComponent<USBCharacter>();
        _player.character.transform.position = new Vector3(0, 5, 0); // Temp.

        // Configure the USBCharacter.
        _player.character.SetColour(_player.color);
        _player.character.name = "Player" + _player.id;

        //LoadoutFactory.AssignLoadout(_player.character, "Base");
        LoadoutFactory.AssignRandomLoadout(_player.character);
    }

}
