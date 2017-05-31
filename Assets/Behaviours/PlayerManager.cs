using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerManager : MonoBehaviour
{
    private Dictionary<int, Player> connected_players = new Dictionary<int, Player>();


    void Awake()
    {
        // Subscribe to Rewired controller connection events.
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        // Add a new player for all connected joysticks at startup.
        AssignAllConnectedJoysticks();
    }


    void Update()
    {
        DropInDropOut();
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
        Player connected_player = connected_players[controller_id];
        connected_player.controllers.ClearAllControllers();

        // Remove the player entry from the list.
        connected_players.Remove(controller_id);
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
        Player connected_player = ReInput.players.GetPlayer(_controller_id);

        connected_player.controllers.AddController(ReInput.controllers.Joysticks[_controller_id], true);
        connected_player.isPlaying = false;

        connected_players.Add(_controller_id, connected_player);
    }


    // Handle drop-in drop-out from user input.
    void DropInDropOut()
    {
        foreach (Player player in connected_players.Values)
        {
            // Handle join command.
            if (player.GetButtonDown("Join") && !player.isPlaying)
            {
                PlayerJoin(player.id);
            }
            // Handle leave command.
            else if (player.GetButtonDown("Leave") && player.isPlaying)
            {
                PlayerLeave(player.id);
            }
        }
    }


    // Called when a player presses JOIN.
    void PlayerJoin(int player_id)
    {
        connected_players[player_id].isPlaying = true;
    }


    // Called when a player presses LEAVE.
    void PlayerLeave(int player_id)
    {
        connected_players[player_id].isPlaying = false;
    }

}
