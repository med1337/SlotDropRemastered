using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShockwave : Projectile
{
    public GameObject particle_effect;

    protected override void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);

        CreateExplosionForce(owning_player.gameObject, origin, 
            properties.effect_radius, properties.knockback_force);

        Destroy(gameObject);
    }

}
