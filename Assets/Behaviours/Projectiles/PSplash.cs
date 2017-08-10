using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSplash : Projectile
{
    public GameObject particle_effect;
    public float stun_chance;
    public float stun_duration;
    public float dist_from_player = 2;
    public float damage_radius = 3;


	void Start()
    {
        this.GetComponent<SphereCollider>().radius = damage_radius;

        if (owner)
            origin = owner.body_group.transform.position;

        Vector3 offset_pos = origin + (facing * (1 + dist_from_player));
        transform.position = offset_pos + (facing * dist_from_player);

        CreateEffect(particle_effect, origin + (facing * 3), origin + (facing * 10));
    }


    void OnTriggerEnter(Collider _other)
    {
        // Only collide with players.
        if (_other.tag != "USBCharacter")
            return;

        USBCharacter character = _other.GetComponent<USBCharacter>();

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
