using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<USBCharacter> alive_characters = new List<USBCharacter>();
    public bool titan_exists { get { return alive_characters.Any(elem => elem.is_titan); } }

    [SerializeField] GameObject usb_character_prefab;
    [SerializeField] string starting_loadout = "Base";
    [SerializeField] bool spawn_ai_with_random_loadout = true;
    [SerializeField] public StateMachine ai_behaviour;
    [Space]

    private List<USBCharacter> alive_ai = new List<USBCharacter>();
    private SpawnAreaCircle spawn_area;


    public bool MorePlayersNeeded()
    {
        if ((PlayerManager.active_player_count <= 1 && GameManager.min_ai == 0) ||
            (PlayerManager.active_player_count == 0 && GameManager.min_ai > 0))
        {
            return true;
        }

        return false;
    }


    public void TitanAllCharacters()
    {
        foreach (USBCharacter character in alive_characters)
            character.BecomeTitan();
    }


    public void KillAllCharacters()
    {
        foreach (USBCharacter character in alive_characters)
            Destroy(character.gameObject);

        GameManager.min_ai = 0;
    }


    void Start()
    {
        spawn_area = GetComponent<SpawnAreaCircle>();
    }


    void Update()
    {
        alive_characters.RemoveAll(elem => elem == null);
        alive_ai.RemoveAll(elem => elem == null);

        if (!GameManager.restarting_scene)
        {
            RespawnPlayers();
            RespawnAI();
        }

        if (Input.GetKeyDown(KeyCode.T))
            TitanAllCharacters();

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
        if (alive_ai.Count < GameManager.min_ai)
            CreateUSBAICharacter();

        if (alive_ai.Count > GameManager.min_ai)
            Destroy(alive_ai[alive_ai.Count - 1].gameObject);
    }


    void Debug()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
            --GameManager.min_ai;

        if (Input.GetKeyDown(KeyCode.RightBracket))
            ++GameManager.min_ai;
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
        USBCharacter character = CreateUSBCharacter("AIDude", Color.white);
        USBAI created_ai = character.gameObject.AddComponent<USBAI>();

        if (ai_behaviour != null)
            created_ai.SetAIBehaviour(ai_behaviour);

        if (spawn_ai_with_random_loadout)
        {
            LoadoutFactory.AssignLoadout(character, "Base");
            LoadoutFactory.AssignRandomLoadout(character);
        }

        alive_ai.Add(character);
    }


    USBCharacter CreateUSBCharacter(string _name, Color _color)
    {
        // Create the USBCharacter.
        USBCharacter character = Instantiate(usb_character_prefab).GetComponent<USBCharacter>();

        // Randomly position the USBCharacter.
        Vector2 random_circle_location = Random.insideUnitCircle * spawn_area.spawn_radius;
        character.transform.position = transform.position +
            new Vector3(random_circle_location.x, 0, random_circle_location.y);

        // Configure the USBCharacter.
        character.SetColour(_color);
        character.name = _name;

        LoadoutFactory.AssignLoadout(character, starting_loadout == "" ? "Base" : starting_loadout);

        character.Init();
        character.Stun(0.9f, false);
        character.Flash(Color.white);

        alive_characters.Add(character);

        return character;
    }



}
