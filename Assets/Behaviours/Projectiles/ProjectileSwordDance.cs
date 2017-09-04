using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSwordDance : Projectile
{
    public float orbit_distance = 5.0f;
    public float rotate_speed = 10.0f;
    public float speed_modifier = 0.5f;
    public float stun_duration = 0.5f;
    public float stun_chance = 10;

    private GameObject orbit_axis;


    void Start()
    {
        if (owner)
        {
            orbit_axis = owner.body_group;
            transform.position = orbit_axis.transform.position;

            if (owner.move_dir.magnitude > 0.05f)
                owner.rigid_body.AddForce(facing * 10000);

            owner.AddSpeedModifier(speed_modifier, lifetime);
        }

        GetComponent<SphereCollider>().center = new Vector3(orbit_distance, 0, 0);
        transform.FindChild("SwordDanceParticle").transform.localPosition = new Vector3(orbit_distance, 0, 0);

        Destroy(this.gameObject, lifetime);
    }


    void Update()
    {
        if (!owner)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = orbit_axis.transform.position;
        transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
    }


    void OnTriggerEnter(Collider _other)
    {
        // Only collide with players.
        if (_other.tag != "USBCharacter")
            return;

        USBCharacter character = _other.GetComponent<USBCharacter>();

        // Don't collide with self.
        if (owner)
        {
            if (character == owner)
                return;
        }

        AudioManager.PlayOneShot("sword_dance_hit");
        character.Damage(damage, (_other.transform.position - transform.position) * 5, owner);

        if (Random.Range(1, 100) <= stun_chance)
            character.Stun(stun_duration);
    }

}