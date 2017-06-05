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
            if (player.state == PlayerState.PLAYING && player.character == null)
            {
                player.character = Instantiate(usb_character).GetComponent<USBCharacter>();
                player.character.transform.position = new Vector3(0, 5, 0);
            }
        }
    }

}
