using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePellet : Projectile
{
    public float pellet_speed = 5.0f;


    void Start()
    {
        if (owner)
            origin = owner.body_group.transform.position + (facing * 2);

        transform.position = origin;
    }


	void Update()
    {
		transform.position += facing * Time.deltaTime * pellet_speed;
	}


    void OnTriggerEnter(Collider _other)
    {
        USBCharacter character = _other.GetComponent<USBCharacter>();

        if (character != null)
        {
            // Don't collide with self.
            if (owner)
            {
                if (character == owner)
                    return;
            }

            character.Damage(damage, origin);
        }

        AudioManager.PlayOneShot("projectile_impact");
        Destroy(this.gameObject);
    }

}
