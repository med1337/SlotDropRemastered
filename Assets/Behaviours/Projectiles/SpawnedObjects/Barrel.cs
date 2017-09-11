using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] AudioSource audio_source;

    public GameObject shockwave_particle;
    public USBCharacter owner;
    public Rigidbody rigid_body;

    public float effect_radius;
    public float knockback_force;
    public float stun_duration;

    public float fuse_time;
    private float timer;


    public void AddForce(Vector3 force)
    {
        rigid_body.AddForce(force);
    }


    void Start()
    {
        audio_source.volume = AudioManager.sfx_volume;
    }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fuse_time)
            Explode();
    }


    void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.layer == LayerMask.NameToLayer("Floor") ||
            (owner != null && _other.transform.GetInstanceID() == owner.transform.GetInstanceID()))
        {
            return;
        }

        Explode();
    }


    void Explode()
    {
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

        AudioManager.PlayOneShot("explosion");
        CameraShake.Shake(0.4f, 0.4f);

        Destroy(this.gameObject);
    }

}
