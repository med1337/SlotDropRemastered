using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePellet : Projectile
{
    [SerializeField] AudioClip whiff_sound;
    [SerializeField] float pellet_speed = 5.0f;
    [SerializeField] int num_bounces = 0;
    [SerializeField] float bounce_radius = 5.0f;
    [SerializeField] float bounce_damage_reduction_factor = 0.5f;
    [SerializeField] bool use_facing_rotation;

    private int bounce_times = 0;
    private USBCharacter last_character_hit;


    void Start()
    {
        if (owner)
            origin = owner.body_group.transform.position + (facing * 2);

        transform.position = origin;
        
        if (use_facing_rotation)
            transform.rotation = Quaternion.LookRotation(-facing);
    }


	void Update()
    {
		transform.position += facing * pellet_speed * Time.deltaTime;
	}


    void OnTriggerEnter(Collider _other)
    {
        USBCharacter character = _other.GetComponent<USBCharacter>();

        if (character != null)
        {
            // Don't collide with self.
            if (owner != null && character == owner)
                return;
        }

        if (character == null)
        {
            AudioManager.PlayOneShot(whiff_sound);
            Destroy(this.gameObject);
        }
        else
        {
            AudioManager.PlayOneShot(hit_sound);
            HandleBounce(character);
        }
    }


    void HandleBounce(USBCharacter _character)
    {
        _character.Damage(damage, origin, owner);
        damage -= Mathf.RoundToInt(damage * bounce_damage_reduction_factor);

        UpdateLastCharacterHit(_character);

        if (bounce_times >= num_bounces)
        {
            Destroy(this.gameObject);
            return;
        }

        USBCharacter bounce_target = null;
        float closest_dist = float.MaxValue;

        var hits = Physics.SphereCastAll(transform.position, bounce_radius, Vector3.down, 0, 1 << LayerMask.NameToLayer("Player"));

        foreach (var hit in hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == owner || character == last_character_hit)
                continue;

            float dist = Vector3.Distance(transform.position, character.body_group.transform.position);

            if (dist > bounce_radius || dist >= closest_dist)
                continue;

            bounce_target = character;
            closest_dist = dist;
        }

        if (bounce_target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            ++bounce_times;

            if (closest_dist <= 2.0f * bounce_target.transform.localScale.x)
                HandleBounce(bounce_target);

            facing = (bounce_target.body_group.transform.position - transform.position).normalized;

            if (use_facing_rotation)
                transform.rotation = Quaternion.LookRotation(-facing);
        }
    }


    void UpdateLastCharacterHit(USBCharacter _character)
    {
        last_character_hit = _character;
        transform.position = origin = _character.body_group.transform.position;

        CancelInvoke("DestroyProjectile");
        Invoke("DestroyProjectile", lifetime);
    }

}
