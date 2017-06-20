using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PArrowHail : Projectile
{
    public GameObject particle_effect;
    public float damage_delay;
    public float stun_chance;
    public float stun_duration;
    public float effect_radius;

    private float timer;
    private bool can_damage;


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);
        Invoke("EnableDamage", 2.0f);
    }


    void Update()
    {
        if (!can_damage)
            return;

        timer += Time.deltaTime;

        if (timer >= damage_delay)
        {
            timer = 0;

            DamageAllInRadius();
        }
    }


    void EnableDamage()
    {
        can_damage = true;
    }


    void DamageAllInRadius()
    {
        RaycastHit[] sphere = Physics.SphereCastAll(transform.position, effect_radius, Vector3.up, 0);

        foreach (var elem in sphere)
        {
            if (elem.collider.tag != "USBCharacter")
                continue;

            USBCharacter character = elem.collider.gameObject.GetComponent<USBCharacter>();

            if (character == null ||
                character == owner)
            {
                continue;
            }

            if (Random.Range(1, 100) < stun_chance)
                character.Stun(stun_duration);

            character.Damage(damage);
        }
    }


}
