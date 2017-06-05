using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, ConnectedPlayer> connected_players = new Dictionary<int, ConnectedPlayer>();


    void Awake()
    {
        // Subscribe to Rewired controller connection events.
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        // Add a new player for all connected joysticks at startup.
        AssignAllConnectedJoysticks();
    }


    void AssignAllConnectedJoysticks()
    {
        foreach (Joystick joystick in ReInput.controllers.Joysticks)
        {
            AddPlayer(joystick.id);
        }
    }


    void AddPlayer(int _controller_id)
    {
        ConnectedPlayer connected_player = new ConnectedPlayer();

        connected_player.input = ReInput.players.GetPlayer(_controller_id);
        connected_player.input.controllers.AddController(ReInput.controllers.Joysticks[_controller_id], true);

        connected_players.Add(_controller_id, connected_player);
    }


    void Update()
    {
        HandlePlayers();
    }


    void OnControllerConnected(ControllerStatusChangedEventArgs _args)
    {
        if (_args.controllerType != ControllerType.Joystick)
            return;

        AddPlayer(_args.controllerId);
    }


    void OnControllerDisconnected(ControllerStatusChangedEventArgs _args)
    {
        int controller_id = _args.controllerId;

        // Remove controller from the player it is attached to.
        ConnectedPlayer connected_player = connected_players[controller_id];
        connected_player.input.controllers.ClearAllControllers();

        // Remove the player entry from the list.
        connected_players.Remove(controller_id);
    }


    // Tick players and handle drop-in / drop-out.
    void HandlePlayers()
    {
        foreach (ConnectedPlayer player in connected_players.Values)
        {
            player.Update();

            if (player.state == PlayerState.JOINING)
            {
                PlayerJoin(player.id);
            }
            else if (player.state == PlayerState.LEAVING)
            {
                PlayerLeave(player.id);
            }
        }
    }


    // Create a character for the player to control and set to playing state.
    void PlayerJoin(int _player_id)
    {
        ConnectedPlayer connected_player = connected_players[_player_id];

        connected_player.state = PlayerState.PLAYING;
    }


    // Remove the player's attached character and set to waiting state.
    void PlayerLeave(int _player_id)
    {
        ConnectedPlayer connected_player = connected_players[_player_id];

        Destroy(connected_player.character.gameObject);
        connected_player.state = PlayerState.WAITING;
    }


    public Dictionary<int, ConnectedPlayer>.ValueCollection GetPlayers()
    {
        return connected_players.Values;
    }

}
