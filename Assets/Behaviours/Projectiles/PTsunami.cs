using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTsunami : Projectile
{
    public GameObject particle_effect;
    public float effect_radius;
    public float knockback_force;
    public float stun_duration;

    public float blast_delay;
    public float blast_spacing;
    public int max_blasts;

    private float timer;
    private float blast_times;


	void Start()
    {
        GetComponent<CapsuleCollider>().radius = effect_radius;

        timer = blast_delay;
    }


    void Update()
    {
        if (blast_times < max_blasts)
        {
            timer += Time.deltaTime;

            if (timer >= blast_delay)
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

        ++blast_times;
        transform.position += facing * blast_spacing;

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

        CameraShake.Shake(0.4f, 0.4f);
    }

}
