using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPellet : Projectile
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
        GameObject.FindObjectOfType<AudioManager>().PlayOneShot("projectile_impact");

        if (_other.tag == "Prop")
        {
            Destroy(this.gameObject);
        }
        
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

        character.Damage(damage, origin);
        Destroy(this.gameObject);
    }

}
