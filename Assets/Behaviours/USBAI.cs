using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class USBAI : MonoBehaviour
{
    private USBCharacter character;
    private float t = 0;


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
        t += Time.deltaTime;
        USBCharacter closest_enemy = null;

        if (t >= 0.2f)
        {
            var enemies = GameObject.FindObjectsOfType<USBCharacter>();
            float closest_distance = float.PositiveInfinity;

            foreach (var enemy in enemies)
            {
                if (enemy == character)
                    continue;

                float dist = Vector3.Distance(enemy.transform.position, transform.position);
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
