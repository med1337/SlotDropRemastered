using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTsunami : Projectile
{
    public GameObject particle_effect;
    public float effect_radius;
    public float knockback_force;
    public float stun_duration;

    public float move_delay;
    public float move_spacing;
    public int max_moves;

    private float timer;
    private float move_times;


	void Start()
    {
        GetComponent<CapsuleCollider>().radius = effect_radius;

        origin += facing * move_spacing;
        transform.position = origin;

        CreateBlast();
    }


    void Update()
    {
        if (move_times < max_moves)
        {
            timer += Time.deltaTime;

            if (timer >= move_delay)
            {
                timer = 0;

                CreateBlast();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void CreateBlast()
    {
        GameObject.FindObjectOfType<AudioManager>().PlayOneShot("water_explosion");

        ++move_times;
        transform.position += facing * move_spacing;

        CreateEffect(particle_effect, transform.position, Vector3.zero);

        var elems = CreateExplosion(owner ? owner.gameObject : null, 
            transform.position, effect_radius, knockback_force);

        foreach (var elem in elems)
        {
            USBCharacter character = elem.collider.gameObject.GetComponent<USBCharacter>();

            if (!character)
                continue;

            if (character != owner)
                character.Stun(stun_duration);
        }

        CameraShake.Shake(knockback_force / 10, knockback_force / 10);
    }

}
