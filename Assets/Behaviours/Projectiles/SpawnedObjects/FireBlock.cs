using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlock : MonoBehaviour
{
    private USBCharacter owner;
    private float timer;
    private int damage;
    private float damage_delay;
    private Vector3 damage_box;
    private Vector3 facing;


    public void Init(USBCharacter _owner, int _damage, float _damage_delay,
        Vector3 _damage_box, Vector3 _facing)
    {
        owner = _owner;
        damage = _damage;
        damage_delay = _damage_delay;
        damage_box = _damage_box;
        facing = _facing;
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= damage_delay)
        {
            timer = 0;

            DamageAllInRadius();
        }
    }


    void DamageAllInRadius()
    {
        RaycastHit[] box = Physics.BoxCastAll(transform.position, damage_box, Vector3.up,
            Quaternion.LookRotation(facing), 0);

        foreach (var elem in box)
        {
            if (elem.collider.tag != "USBCharacter")
                continue;

            USBCharacter character = elem.collider.gameObject.GetComponent<USBCharacter>();

            if (character == owner)
                continue;

            character.Damage(damage, owner);
        }
    }

}