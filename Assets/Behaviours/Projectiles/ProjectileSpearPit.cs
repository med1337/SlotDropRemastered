using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpearPit : Projectile
{
    [SerializeField] float damage_delay = 0.5f;
    [SerializeField] Vector3 damage_box_size;
    [SerializeField] float stun_duration = 0.5f;

    private float timer;


    void Start()
    {
        CameraShake.Shake(0.1f, 0.1f);
    }


    void Update()
    {
        float prev_timer = timer;
        timer += Time.deltaTime;

        if (timer >= damage_delay &&
            prev_timer < damage_delay)
        {
            DealDamage();
        }
    }


    void DealDamage()
    {
        var hits = Physics.BoxCastAll(origin, damage_box_size / 2, Vector3.down,
            Quaternion.identity, 0, 1 << LayerMask.NameToLayer("Player"));

        foreach (var hit in hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == null || character == owner)
                continue;

            character.Damage(damage, owner);
            character.Stun(stun_duration);
        }

        CameraShake.Shake(0.4f, 0.4f);
        this.enabled = false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(origin, damage_box_size / 2);
    }

}
