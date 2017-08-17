using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
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

        // Debug.
        DebugSpawnAI();
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


    void DebugSpawnAI()
    {
        if (!Input.GetKeyDown(KeyCode.I))
            return;

        USBCharacter character = CreateUSBCharacter("AIDude", Color.black);
        character.gameObject.AddComponent<USBAI>();
    }


    bool PlayerAwaitingRespawn(ConnectedPlayer _player)
    {
        return _player.state == PlayerState.PLAYING && _player.character == null;
    }


    void RespawnPlayer(ConnectedPlayer _player)
    {
        _player.character = CreateUSBCharacter("Player" + _player.id.ToString(), _player.color);
    }


    USBCharacter CreateUSBCharacter(string _name, Color _color)
    {
        // Create and position the USBCharacter.
        USBCharacter character = Instantiate(usb_character_prefab).GetComponent<USBCharacter>();
        character.transform.position = new Vector3(0, 5, 0); // Temp.

        // Configure the USBCharacter.
        character.SetColour(_color);
        character.name = _name;

        LoadoutFactory.AssignRandomLoadout(character);

        return character;
    }

}
