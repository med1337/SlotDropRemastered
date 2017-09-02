using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class USBAI : MonoBehaviour
{
    private USBCharacter character;
    private float timer = 0;
    private USBCharacter closest_enemy;


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
        }
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 0.2f)
        {
            timer = 0;
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

            character.Attack();
            character.SlotDrop();
        }
    }

}
