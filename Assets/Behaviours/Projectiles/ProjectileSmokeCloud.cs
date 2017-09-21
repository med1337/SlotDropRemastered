using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSmokeCloud : Projectile
{
    [SerializeField] GameObject particle_prefab;
    [SerializeField] float effect_radius = 5;
    [SerializeField] float lift_force = 5;
    [SerializeField] float stun_duration = 0.5f;


    void Start()
    {
        CreateEffect(particle_prefab, origin, Vector3.zero);
        CameraShake.Shake(0.3f, 0.3f);

        var hits = Physics.SphereCastAll(origin, effect_radius, Vector3.down, 0, ~LayerMask.NameToLayer("Projectile"));

        foreach (var hit in hits)
        {
            if (hit.rigidbody != null)
                hit.rigidbody.AddForce(new Vector3(0, lift_force * 1000, 0));

            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == null || character == owner)
                continue;

            character.Damage(damage, owner);
            character.Stun(stun_duration);
        }

        Destroy(this.gameObject);
    }

}
