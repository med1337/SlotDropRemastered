using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject usb_character;


    void Start()
    {
		
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
        _player.character = Instantiate(usb_character).GetComponent<USBCharacter>();
        _player.character.transform.position = new Vector3(0, 5, 0); // Temp.

        // Configure the USBCharacter.
        _player.character.SetColour(_player.color);
        _player.character.name = "Player" + _player.id;

        GameObject.FindObjectOfType<LoadoutFactory>().AssignRandomLoadout(_player.character);
    }

}
