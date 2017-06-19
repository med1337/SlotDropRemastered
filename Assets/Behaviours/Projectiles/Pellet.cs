using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePellet : Projectile
{

    protected override void Start()
    {
        if (owning_player != null)
            origin = owning_player.transform.FindChild("BodyParts").position + (facing * 2);

        transform.position = origin;
    }


	protected override void Update()
    {
		transform.position += facing * Time.deltaTime * properties.projectile_speed;
	}


    protected override void OnTriggerEnter(Collider other)
    {
        GameObject.FindObjectOfType<AudioManager>().PlayOneShot("projectile_impact");

        if (other.tag == "Prop")
        {
            Destroy(gameObject);
        }
        
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

        colliding_player.Damage(properties.damage);
        Destroy(gameObject);
    }

}
