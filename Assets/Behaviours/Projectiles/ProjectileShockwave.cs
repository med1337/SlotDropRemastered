using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;

public class ProjectileShockwave : Projectile
{
    public GameObject particle_effect;
    public float effect_radius;
    public float knockback_force;
    public float stun_duration;

    private float wave_speed;
    private SphereCollider sphere_collider;
    private List<USBCharacter> affected_characters = new List<USBCharacter>();
    private List<Rigidbody> affected_bodies = new List<Rigidbody>();


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);

        var elems = Physics.SphereCastAll(origin, effect_radius / 10, Vector3.down, 0,
            1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Prop") | 1 << LayerMask.NameToLayer("PhysicsProjectile"));

        foreach (var elem in elems)
        {
            AffectRigidBody(elem.rigidbody);
        }

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
        AffectRigidBody(_other.GetComponent<Rigidbody>());
    }


    void AffectRigidBody(Rigidbody _body)
    {
        if (_body == null || _body.tag == "Sparkle")
            return;

        if (owner != null && owner.rigid_body != _body && !affected_bodies.Contains(_body))
        {
            _body.AddExplosionForce(knockback_force * 1000, origin, effect_radius);
            affected_bodies.Add(_body);
        }

        USBCharacter character = _body.GetComponent<USBCharacter>();

        if (character != null && character != owner)
        {
            if (!affected_characters.Contains(character))
            {
                character.Damage(damage, owner);
                character.Stun(stun_duration);

                affected_characters.Add(character);
            }
        }
    }

}
