using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrowHail : Projectile
{
    public GameObject particle_effect;
    public float delay_before_active;
    public float damage_interval;
    public float stun_chance;
    public float stun_duration;
    public float slow_modifier;
    public float effect_radius;
    public bool sink_after_lifetime;
    public float sink_speed;

    private float timer;
    private float damage_timer;
    private bool can_damage;
    private GameObject created_particle;


    void Start()
    {
        if (sink_after_lifetime)
            CancelInvoke("DestroyProjectile");

        created_particle = Instantiate(particle_effect, origin, Quaternion.identity);
        created_particle.transform.Rotate(-90, 0, Random.Range(0, 360));

        if (delay_before_active > 0)
            Invoke("EnableDamage", delay_before_active);
        else
            EnableDamage();
    }


    void Update()
    {
        if (!can_damage)
            return;

        timer += Time.deltaTime;
        damage_timer += Time.deltaTime;

        if (damage_timer >= damage_interval)
        {
            damage_timer = 0;

            DamageAllInRadius();
        }

        // Sink.
        if (sink_after_lifetime && timer >= lifetime)
        {
            if (created_particle == null)
                return;

            created_particle.transform.position -= new Vector3(0, sink_speed, 0) * Time.deltaTime;

            if (timer >= lifetime + 0.5f)
            {
                Destroy(this.gameObject);
                Destroy(created_particle.gameObject);
            }
        }
    }


    void EnableDamage()
    {
        can_damage = true;
    }


    void DamageAllInRadius()
    {
        RaycastHit[] sphere = Physics.SphereCastAll(transform.position, effect_radius,
            Vector3.down, 0, 1 << LayerMask.NameToLayer("Player"));

        foreach (var elem in sphere)
        {
            USBCharacter character = elem.collider.gameObject.GetComponent<USBCharacter>();

            if (character == owner)
                continue;

            if (stun_chance != 0 && Random.Range(1, 100) < stun_chance)
                character.Stun(stun_duration);

            character.Damage(damage, owner);

            if (slow_modifier != 0)
                character.AddSpeedModifier(slow_modifier, damage_interval);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, effect_radius);
    }


}
