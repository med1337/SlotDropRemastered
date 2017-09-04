﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBAI : MonoBehaviour
{
    private USBCharacter character;
    private float waypoint_timer = 0;
    private USBCharacter closest_enemy;

    private const float MIN_BASIC_USE = 1;
    private const float MAX_BASIC_USE = 2;
    private const float MIN_SPECIAL_USE = 2;
    private const float MAX_SPECIAL_USE = 4;

    private float current_basic_delay;
    private float current_special_delay;


    void Awake()
    {
        character = GetComponent<USBCharacter>();

        if (character == null)
        {
            Destroy(this);
        }
        else
        {
            LoadoutFactory.AssignRandomLoadout(character);
            character.AddSpeedModifier(0.75f, float.MaxValue);
        }
    }


    void Update()
    {
        HandleMovement();
        HandleBasic();
        HandleSpecial();
    }


    void HandleMovement()
    {
        waypoint_timer += Time.deltaTime;

        if (waypoint_timer >= 0.2f)
        {
            waypoint_timer = 0;
            var enemies = GameManager.scene.respawn_manager.alive_characters;
            float closest_distance = float.PositiveInfinity;

            foreach (var enemy in enemies)
            {
                if (enemy == character)
                    continue;

                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist >= closest_distance)
                    continue;

                closest_distance = dist;
                closest_enemy = enemy;
            }
        }

        if (closest_enemy != null)
        {
            var dist = (closest_enemy.transform.position - transform.position);
            dist.y = 0;

            if (dist.magnitude > 3)
                character.Move(dist.normalized);
            else
                character.Face(dist.normalized);
        }
    }


    void HandleBasic()
    {
        if (closest_enemy == null && current_basic_delay != 0)
            current_basic_delay = 0;

        if (Time.time < current_basic_delay)
            return;

        current_basic_delay = Time.time + Random.Range(MIN_BASIC_USE, MAX_BASIC_USE);
        character.Attack();
    }


    void HandleSpecial()
    {
        if (closest_enemy == null && current_special_delay != 0)
            current_special_delay = 0;

        if (Time.time < current_special_delay)
            return;

        current_special_delay = Time.time + Random.Range(MIN_SPECIAL_USE, MAX_SPECIAL_USE);
        character.SlotDrop();
    }

}
