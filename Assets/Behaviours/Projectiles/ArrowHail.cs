using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowHail : Projectile
{
    public GameObject particle_effect;
    public int damage = 10;
    public float damage_delay = 0.5f;
    public float stun_chance = 0;
    public float stun_duration = 0.5f;
    public float effect_radius = 3.0f;

    private List<Collider> colliding_objects = new List<Collider>();
    private float timer;
    private bool can_damage;


    void Start()
    {
        CreateEffect(particle_effect, origin, Vector3.zero);
        Invoke("EnableDamage", 2.0f);
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= damage_delay)
        {
            timer = 0;

            if (!can_damage)
                return;

            DamageAllInRadius();
        }
    }


    void EnableDamage()
    {
        can_damage = true;
    }


    void DamageAllInRadius()
    {
        RaycastHit[] sphere = Physics.SphereCastAll(transform.position, effect_radius, Vector3.zero, 0);

        foreach (var elem in sphere)
        {
            if (elem.collider.tag != "USBCharacter")
                continue;

            USBCharacter character = elem.collider.gameObject.GetComponent<USBCharacter>();

            if (character == null ||
                character == owner)
                continue;

            if (Random.Range(1, 100) < stun_chance)
                character.Stun(stun_duration);

            character.Damage(damage);
        }
    }


}
