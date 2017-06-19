using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSwordDance : Projectile
{
    public float orbit_distance = 5.0f;
    public float rotate_speed = 10.0f;

    private GameObject orbit_axis;


    protected override void Start()
    {
        if (owning_player != null)
        {
            orbit_axis = owning_player.transform.FindChild("BodyParts").gameObject;
            transform.position = orbit_axis.transform.position;

            owning_player.move_speed_modifier = 0.5f;
        }

        GetComponent<SphereCollider>().center = new Vector3(orbit_distance, 0, 0);
        transform.FindChild("SwordDanceParticle").transform.localPosition = new Vector3(orbit_distance, 0, 0);

        Destroy(gameObject, properties.lifetime);
    }


    protected override void Update()
    {
        if (owning_player == null || orbit_axis == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = orbit_axis.transform.position;
        transform.Rotate(Vector3.up * Time.deltaTime * rotate_speed);
    }


    protected override void OnTriggerEnter(Collider other)
    {
        // Only collide with players.
        if (other.tag != "Player")
            return;

        PlayerController colliding_player = other.GetComponent<PlayerController>();

        // Don't collide with self.
        if (owning_player != null)
        {
            if (colliding_player == owning_player)
                return;
        }

        GameObject.FindObjectOfType<AudioManager>().PlayOneShot("sword_dance_hit");
        colliding_player.Damage(properties.damage);
    }


    private void OnDestroy()
    {
        // Reset player speed.
        if (owning_player != null)
            owning_player.move_speed_modifier = 1;
    }
}