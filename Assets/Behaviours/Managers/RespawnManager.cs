using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<USBCharacter> alive_characters = new List<USBCharacter>();

    [SerializeField] GameObject usb_character_prefab;
    [SerializeField] Vector3 spawn_point;

    private List<USBCharacter> alive_ai = new List<USBCharacter>();
    private int min_ai;


    void Start()
    {

    }


    void Update()
    {
        alive_characters.RemoveAll(elem => elem == null);
        alive_ai.RemoveAll(elem => elem == null);

        RespawnPlayers();
        RespawnAI();

        // Debug.
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (USBCharacter character in alive_characters)
                character.BecomeTitan();
        }

        Debug();
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


    void RespawnAI()
    {
        if (alive_ai.Count < min_ai)
            CreateUSBAICharacter();

        if (alive_ai.Count > min_ai)
            Destroy(alive_ai[alive_ai.Count - 1].gameObject);
    }


    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            --min_ai;

        if (Input.GetKeyDown(KeyCode.RightBracket))
            ++min_ai;

        min_ai = Mathf.Clamp(min_ai, 0, 32);
    }


    bool PlayerAwaitingRespawn(ConnectedPlayer _player)
    {
        return _player.state == PlayerState.PLAYING && _player.character == null;
    }


    void RespawnPlayer(ConnectedPlayer _player)
    {
        _player.character = CreateUSBCharacter("Player" + _player.id.ToString(), _player.color);
    }


    void CreateUSBAICharacter()
    {
        USBCharacter character = CreateUSBCharacter("AIDude", Color.black);
        character.gameObject.AddComponent<USBAI>();

        Vector2 random_circle_location = Random.insideUnitCircle * 30;

        character.transform.position = new Vector3(character.transform.position.x + random_circle_location.x,
            character.transform.position.y, character.transform.position.z + random_circle_location.y);

        alive_ai.Add(character);
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

        return character;
    }



}
