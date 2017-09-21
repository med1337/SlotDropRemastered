using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class DebugPanelManager : MonoBehaviour
{
    [SerializeField] List<MenuOption> menu_options;
    [SerializeField] float value_step = 0.05f;
    
    private ConnectedPlayer controlling_player;
    private const float AXIS_THRESHOLD = 0.5f;
    private int selected_index;


    public void Activate(ConnectedPlayer _activating_player = null)
    {
        this.gameObject.SetActive(true);
        controlling_player = _activating_player;

        SelectFirst();
    }


    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }


    void Awake()
    {
        foreach (MenuOption option in menu_options)
        {
            option.Deselect();

            if (option.option_type == MenuOptionType.MUSIC_VOLUME)
                option.value = AudioManager.music_volume;
            else if (option.option_type == MenuOptionType.SFX_VOLUME)
                option.value = AudioManager.sfx_volume;
        }
    }


    void SelectFirst()
    {
        foreach (MenuOption option in menu_options)
            option.Deselect();

        selected_index = 0;
        menu_options[0].Select();
    }


    void Update()
    {
        if (controlling_player != null)
        {
            float horizontal = controlling_player.input.GetAxisRaw("Horizontal");
            float vertical = controlling_player.input.GetAxisRaw("Vertical");

            float horizontal_prev = controlling_player.input.GetAxisRawPrev("Horizontal");
            float vertical_prev = controlling_player.input.GetAxisRawPrev("Vertical");

            AxisTapEvents(horizontal, vertical, horizontal_prev, vertical_prev);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E) ||
            (controlling_player != null && controlling_player.input.GetButtonDown("SlotDrop")))
        {
            menu_options[selected_index].Press();
        }

        if (controlling_player != null && (controlling_player.input.GetButtonDown("Back") ||
                controlling_player.input.GetButtonDown("B")))
        {
            this.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.W))
            PrevMenuOption();

        if (Input.GetKeyDown(KeyCode.S))
            NextMenuOption();

        if (Input.GetKeyDown(KeyCode.D))
        {
            menu_options[selected_index].AdjustValue(value_step);
            ChangeVolume(menu_options[selected_index].value);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            menu_options[selected_index].AdjustValue(-value_step);
            ChangeVolume(menu_options[selected_index].value);
        }
    }


    void AxisTapEvents(float _horizontal, float _vertical,
        float _horizontal_prev, float _vertical_prev)
    {
        // Horizontal Axis.
        if (_horizontal >= AXIS_THRESHOLD &&
            _horizontal_prev < AXIS_THRESHOLD)
        {
            menu_options[selected_index].AdjustValue(value_step);
            ChangeVolume(menu_options[selected_index].value);
        }

        if (_horizontal <= -AXIS_THRESHOLD &&
            _horizontal_prev > -AXIS_THRESHOLD)
        {
            menu_options[selected_index].AdjustValue(-value_step);
            ChangeVolume(menu_options[selected_index].value);
        }

        // Vertical Axis.
        if (_vertical >= AXIS_THRESHOLD &&
            _vertical_prev < AXIS_THRESHOLD)
        {
            PrevMenuOption();
        }

        if (_vertical <= -AXIS_THRESHOLD &&
            _vertical_prev > -AXIS_THRESHOLD)
        {
            NextMenuOption();
        }
    }


    void ChangeVolume(float _value)
    {
        switch (menu_options[selected_index].option_type)
        {
            case MenuOptionType.MUSIC_VOLUME: SetMusicVolume(_value); break;
            case MenuOptionType.SFX_VOLUME: SetSFXVolume(_value); break;
        }
    }


    void NextMenuOption()
    {
        menu_options[selected_index].Deselect();
        ++selected_index;

        if (selected_index >= menu_options.Count)
            selected_index = menu_options.Count - 1;

        menu_options[selected_index].Select();
    }


    void PrevMenuOption()
    {
        menu_options[selected_index].Deselect();
        --selected_index;

        if (selected_index < 0)
            selected_index = 0;

        menu_options[selected_index].Select();
    }


    void OnDisable()
    {
        if (controlling_player != null)
            controlling_player.in_menu = false;

        controlling_player = null;
    }


    void SetMusicVolume(float _volume)
    {
        AudioManager.music_volume = _volume;
    }


    void SetSFXVolume(float _volume)
    {
        AudioManager.sfx_volume = _volume;
    }


    void AddAI()
    {
        ++GameManager.min_ai;
    }


    void RemoveAI()
    {
        --GameManager.min_ai;
    }


    void QuitGame()
    {
        Application.Quit();
    }

}
