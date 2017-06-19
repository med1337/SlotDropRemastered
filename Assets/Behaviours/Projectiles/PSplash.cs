using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSplash : Projectile
{
    public GameObject particle_effect;
    public float stun_chance = 0;
    public float stun_duration = 0;
    public float dist_from_player = 2;


	void Start()
    {
        if (owner)
            origin = owner.body_group.transform.position;

        Vector3 offset_pos = origin + (facing * (1 + dist_from_player));

        CreateEffect(particle_effect, offset_pos, offset_pos + facing);

        transform.position = offset_pos + (facing * dist_from_player);
    }


    void OnTriggerEnter(Collider other)
    {
        // Only collide with players.
        if (other.tag != "USBCharacter")
            return;

        USBCharacter character = other.GetComponent<USBCharacter>();

        // Don't collide with self.
        if (owner)
        {
            if (character == owner)
                return;
        }

        if (Random.Range(0, 100) < stun_chance)
            character.Stun(stun_duration);

        character.Damage(damage);
    }

}
