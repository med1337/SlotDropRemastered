using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShockwave : Projectile
{
    public GameObject particle_effect;
    public float effect_radius;
    public float knockback_force;


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);
        CreateExplosion(owner.gameObject, origin, effect_radius, knockback_force);

        CameraShake.Shake(0.4f, 0.4f);

        Destroy(gameObject);
    }

}
