using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTitanWave : BaseProjectile
{
    public GameObject particle_effect;


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);

        CreateExplosion(owning_player.gameObject, origin, 
            properties.effect_radius, properties.knockback_force);

        Destroy(gameObject);
    }

}
