using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject usb_character;
    public PlayerManager player_manager;


    void Start()
    {
		
    }


    void Update()
    {
        if (player_manager)
        {
            RespawnPlayers();
        }
    }


    void RespawnPlayers()
    {
        foreach (var player in player_manager.GetPlayers())
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
        _player.character = Instantiate(usb_character).GetComponent<USBCharacter>();
        _player.character.transform.position = new Vector3(0, 5, 0); // Temp.

        _player.character.SetColour(_player.color);
        _player.character.name = "Player" + _player.id;
    }

}
