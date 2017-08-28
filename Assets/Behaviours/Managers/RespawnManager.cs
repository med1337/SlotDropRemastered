using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] GameObject usb_character_prefab;
    [SerializeField] Vector3 spawn_point;
    public static List<USBCharacter> alive_characters = new List<USBCharacter>(); 
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
        alive_characters.RemoveAll(item => item == null);
        RespawnPlayers();

        // Debug.
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (USBCharacter character in alive_characters)
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

        Vector2 random_circle_location = Random.insideUnitCircle * 30;

        character.transform.position = new Vector3(character.transform.position.x + random_circle_location.x,
            character.transform.position.y, character.transform.position.z + random_circle_location.y);
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

        LoadoutFactory.AssignLoadout(character, "Base");

        character.Init();
        character.Stun(0.9f, false);
        character.Flash();

        alive_characters.Add(character);
        listener_module.NotifyListeners("AddFocusedObject", character.gameObject);

        return character;
    }



}
