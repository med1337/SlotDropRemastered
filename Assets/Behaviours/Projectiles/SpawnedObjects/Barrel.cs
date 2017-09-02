using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject shockwave_particle;
    public USBCharacter owner;
    public Rigidbody rigid_body;

    public float torque_x = 15.0f;
    public float torque_z = 12.0f;

    public float effect_radius;
    public float knockback_force;
    public float stun_duration;


    public void AddForce(Vector3 force)
    {
        rigid_body.AddForce(force);
    }


    void FixedUpdate()
    {
        rigid_body.AddTorque(new Vector3(torque_x, 0, torque_z));
    }


    void OnTriggerEnter(Collider _other)
    {
        if (_other.tag == "Barrel" ||
            _other.gameObject.layer == LayerMask.NameToLayer("Projectile") ||
            (owner != null && _other.transform.GetInstanceID() == owner.transform.GetInstanceID()))
        {
            return;
        }

        AudioManager.PlayOneShot("explosion");
        CameraShake.Shake(0.4f, 0.4f);

        Projectile.CreateEffect(shockwave_particle, transform.position, Vector3.zero);
        RaycastHit[] elems = Physics.SphereCastAll(transform.position, effect_radius,
            Vector3.down, 0, 1 << LayerMask.NameToLayer("Player"));

        foreach (var elem in elems)
        {
            USBCharacter character = elem.collider.GetComponent<USBCharacter>();

            if (character == owner)
                continue;

            character.Stun(stun_duration);
        }

        Destroy(this.gameObject);
    }

}
