using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float cooldown;
    public float lifetime;
    public AudioClip activation_sound;

    protected USBCharacter owner;
    protected Vector3 origin;
    protected Vector3 facing;


    public void Init(USBCharacter _owner)
    {
        owner = _owner;
        origin = _owner.transform.position;
        facing = _owner.last_facing;

        Destroy(this.gameObject, lifetime);
    }


    // Instantiates a particle prefab and attaches a garbage collection script to it.
    public static void CreateEffect(GameObject _particle_prefab, Vector3 _position, Vector3 _direction)
    {
        GameObject particle = Instantiate(_particle_prefab);

        particle.transform.position = _position;

        if (_direction != Vector3.zero)
            particle.transform.LookAt(_direction);

        var particle_system = particle.GetComponent<ParticleSystem>();

        if (particle_system.collision.enabled)
            particle_system.collision.SetPlane(0, GameObject.Find("Floor").transform);

        particle.AddComponent<TempParticle>();
    }


    // Applies an explosion force to all objects in the radius. Does not affect creator by default.
    public static RaycastHit[] CreateExplosion(GameObject _owner, Vector3 _position, 
        float _radius, float _force, bool _affect_creator = false)
    {
        RaycastHit[] sphere = Physics.SphereCastAll(_position, _radius, Vector3.down, 0);

        foreach (var elem in sphere)
        {
            Rigidbody collided_body = elem.rigidbody;

            if (collided_body == null)
                continue;

            // Don't affect creator.
            if (_owner != null && !_affect_creator)
            {
                if (collided_body == _owner.GetComponent<Rigidbody>())
                    continue;
            }

            collided_body.AddExplosionForce(_force * 1000, _position, _radius);
        }

        return sphere;
    }

}
