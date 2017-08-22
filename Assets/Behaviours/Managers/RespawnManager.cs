using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] GameObject usb_character_prefab;
    [SerializeField] Vector3 spawn_point;
    public static List<USBCharacter> current_players = new List<USBCharacter>(); 
    public static ListenerModule listener_module = new ListenerModule();
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
        current_players.RemoveAll(item => item == null);
        RespawnPlayers();

        // Debug.
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (USBCharacter character in current_players)
                character.BecomeTitan();
        }

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
        character.transform.position = spawn_point;

        // Configure the USBCharacter.
        character.SetColour(_color);
        character.name = _name;

        LoadoutFactory.AssignRandomLoadout(character);
        character.Init();
        character.Stun(0.9f);
        character.Flash();

        current_players.Add(character);
        listener_module.NotifyListeners("AddFocusedObject", character.gameObject);

        return character;
    }



}
