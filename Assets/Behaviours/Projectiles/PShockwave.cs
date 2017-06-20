using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PShockwave : Projectile
{
    public GameObject particle_effect;
    public float effect_radius = 5.0f;
    public float knockback_force = 5.0f;


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);
        CreateExplosion(owner.gameObject, origin, effect_radius, knockback_force);

        CameraShake.Shake(0.4f, 0.4f);

        Destroy(gameObject);
    }

}
