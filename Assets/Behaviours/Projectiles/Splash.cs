using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSplash : Projectile
{
    public GameObject particle_effect;
    public float stun_chance = 0;
    public float dist_from_player = 2;

	protected override void Start()
    {
        if (owning_player != null)
            origin = owning_player.transform.FindChild("BodyParts").position;

        Vector3 offset_pos = origin + (facing * (dist_from_player + 1));

        CreateEffect(particle_effect, offset_pos, offset_pos + facing);

        transform.position = offset_pos + (facing * dist_from_player);
    }


    protected override void OnTriggerEnter(Collider other)
    {
        // Only collide with players.
        if (other.tag != "Player")
            return;

        PlayerController colliding_player = other.GetComponent<PlayerController>();

        // Don't collide with self.
        if (owning_player != null)
        {
            if (colliding_player == owning_player)
                return;
        }

        if (Random.Range(1, 100) < stun_chance)
            colliding_player.Stun(properties.stun_duration);

        colliding_player.Damage(properties.damage);
    }

}
