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

        var hits = Physics.SphereCastAll(origin, effect_radius, Vector3.down, 0, 1 << LayerMask.NameToLayer("Player"));

        foreach (var hit in hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == null || character == owner)
                continue;;

            character.Damage(damage);
            character.Stun(stun_duration);

            character.rigid_body.AddForce(new Vector3(0, lift_force * 1000, 0));
        }

        Destroy(this.gameObject);
    }

}
