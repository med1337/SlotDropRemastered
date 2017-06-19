using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTsunami : Projectile
{
    public GameObject particle_effect;
    public float move_delay;
    public float move_spacing;
    public int max_moves;

    private float timer;
    private float move_times;

	protected override void Start()
    {
        origin = owning_player != null ? 
            owning_player.transform.FindChild("BodyParts").position + (facing * move_spacing) : transform.position;

        GetComponent<CapsuleCollider>().radius = properties.effect_radius;

        CreateBlast();
        
        if (owning_player != null)
            transform.position = origin;
    }


    protected override void Update()
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

        var elems = CreateExplosionForce(owning_player != null ? owning_player.gameObject : null, 
            transform.position, properties.effect_radius, properties.knockback_force);

        foreach (var elem in elems)
        {
            PlayerController player = elem.collider.gameObject.GetComponent<PlayerController>();

            if (player == null)
                continue;

            if (player != owning_player)
                player.Stun(properties.stun_duration);
        }

        CameraShake.instance.ShakeCamera(properties.camera_shake_strength, properties.camera_shake_duration);
    }

}
