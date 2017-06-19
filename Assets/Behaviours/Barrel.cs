using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject shockwave_particle;
    public USBCharacter owner;
    public Rigidbody rigid_body;


    void FixedUpdate()
    {
        rigid_body.AddTorque(new Vector3(15, 0, 12));
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Barrel")
            return;

        GameObject.FindObjectOfType<AudioManager>().PlayOneShot("explosion");

        CameraShake.Shake(0.4f, 0.4f);

        Projectile.CreateEffect(shockwave_particle, transform.position, Vector3.zero);
        RaycastHit[] elems = Projectile.CreateExplosion(gameObject, transform.position, 5, 0);

        foreach (var elem in elems)
        {
            USBCharacter character = elem.collider.GetComponent<USBCharacter>();

            if (character == null)
                continue;

            if (character == owner)
                continue;

            character.Stun(2.0f);
        }

        Destroy(gameObject);
    }


    public void AddForce(Vector3 force)
    {
        rigid_body.AddForce(force);
    }

}
