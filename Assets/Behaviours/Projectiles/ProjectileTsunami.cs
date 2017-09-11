using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTsunami : Projectile
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
        ++blast_times;
        transform.position += facing * blast_spacing;

        if (!PositionValid())
            return;

        CreateEffect(particle_effect, transform.position, Vector3.zero);

        var elems = Projectile.CreateExplosion(owner != null? owner.gameObject : null,
            transform.position, effect_radius, knockback_force);

        foreach (var elem in elems)
        {
            USBCharacter character = elem.collider.GetComponent<USBCharacter>();

            if (!character)
                continue;

            if (character != owner)
                character.Stun(stun_duration);
        }

        AudioManager.PlayOneShot(hit_sound);
        CameraShake.Shake(0.4f, 0.4f);
    }


    bool PositionValid()
    {
        var hits = Physics.RaycastAll(transform.position + new Vector3(0, 5, 0), -Vector3.up, LayerMask.NameToLayer("Floor"));

        return hits.Length > 0;
    }

}
