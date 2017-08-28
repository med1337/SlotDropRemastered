using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShockwave : Projectile
{
    public GameObject particle_effect;
    public float effect_radius;
    public float knockback_force;

    private float wave_speed;
    private SphereCollider sphere_collider;


    void Start()
    {
        origin = owner.transform.Find("Shadow").position;
        origin.y = 0;

        CreateEffect(particle_effect, origin, Vector3.zero);
        CreateExplosion(owner.gameObject, origin, effect_radius, knockback_force);

        wave_speed = particle_effect.GetComponent<ParticleSystem>().main.startSpeed.constantMax;
        sphere_collider = GetComponent<SphereCollider>();

        CameraShake.Shake(0.4f, 0.4f);
    }


    void Update()
    {
        sphere_collider.radius += wave_speed * Time.deltaTime;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(origin, sphere_collider.radius);
    }


    void OnTriggerEnter(Collider _other)
    {
        Rigidbody rb = _other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (owner != null && owner.rigid_body != rb)
                rb.AddExplosionForce(knockback_force, origin, effect_radius);
        }

        USBCharacter character = _other.GetComponent<USBCharacter>();

        if (character != null && character != owner && damage != 0)
        {
            character.Damage(damage);
        }
    }

}
