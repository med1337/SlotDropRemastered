using System.Collections;
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

    private const float TIME_TO_IDLE = 20;
    private float idle_timer;

    private float horizontal;
    private float vertical;


    public void Update()
    {
        HandleDropIn();
        HandleIdle();

        ControlCharacter();

#if UNITY_EDITOR
        DebugCheats();
#endif
}


    void HandleDropIn()
    {
		if (input.GetButtonDown("DropIn"))
        {
            if (state == PlayerState.WAITING)
            {
                state = PlayerState.JOINING;
                idle_timer = 0;
            }
            else if (state == PlayerState.PLAYING)
            {
                state = PlayerState.LEAVING;
                idle_timer = 0;
            }
        }
    }


    void HandleIdle()
    {
        if (state != PlayerState.PLAYING)
            return;

        idle_timer += Time.unscaledDeltaTime;

        if (idle_timer >= TIME_TO_IDLE)
        {
            state = PlayerState.LEAVING;
            idle_timer = 0;
        }
    }


    void ControlCharacter()
    {
        horizontal = input.GetAxis("Horizontal");
        vertical = input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
            idle_timer = 0;

        if (character != null)
        {
            character.SetFaceLocked(input.GetButton("FaceLock"));
            character.Move(new Vector3(horizontal, 0, vertical));
            character.character_indicator.SetActive(input.GetButton("Back"));
        }

        if (input.GetButton("Attack"))
        {
            if (character != null)
                character.Attack();

            WakePlayer();
        }

        if (input.GetButtonDown("SlotDrop"))
        {
            if (character != null)
                character.SlotDrop();

            WakePlayer();
        }

        if (input.GetButtonDown("Y"))
            WakePlayer();

        if (input.GetButtonDown("B"))
            WakePlayer();
    }


    void WakePlayer()
    {
        if (state != PlayerState.WAITING)
            return;

        state = PlayerState.JOINING;
        idle_timer = 0;
    }


    void DebugCheats()
    {
        if (!GameManager.cheats_enabled || !input.GetButton("Back"))
            return;

        if (character != null)
        {
            if (input.GetButton("Down"))
                DebugLoadoutSelectOne();

            if (input.GetButton("Right"))
                DebugLoadoutSelectTwo();

            if (input.GetButton("Left"))
                DebugPlayerStats();
        }

        if (input.GetButton("Up"))
        {
            DebugSystem();
        }
    }


    void DebugLoadoutSelectOne()
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


    void DebugLoadoutSelectTwo()
    {
        if (input.GetButtonDown("Attack"))
            LoadoutFactory.AssignLoadout(character, "Logger");

        if (input.GetButtonDown("Y"))
            LoadoutFactory.AssignLoadout(character, "Egyptian");
    }

    //put any stat related cheats here
    void DebugPlayerStats()
    {
        if (input.GetButtonDown("FaceLock"))
            character.Heal(99999);

        if (input.GetButtonDown("RB"))
            character.stats.target_score += 50;
    }


    void DebugSystem()
    {
        if (input.GetButtonDown("FaceLock"))
        {
            var respawn_manager = GameManager.scene.respawn_manager;

            if (respawn_manager == null)
                return;

            --respawn_manager.min_ai;
        }
        else if (input.GetButtonDown("RB"))
        {
            var respawn_manager = GameManager.scene.respawn_manager;

            if (respawn_manager == null)
                return;

            ++respawn_manager.min_ai;
        }
        else if (input.GetButtonDown("SlotDrop"))
        {
            var respawn_manager = GameManager.scene.respawn_manager;

            if (respawn_manager == null)
                return;

            respawn_manager.TitanAllCharacters();
        }
    }

}
