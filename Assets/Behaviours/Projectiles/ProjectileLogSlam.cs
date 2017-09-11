using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogSlam : Projectile
{
    [SerializeField] Transform box_cast_point;
    [SerializeField] Vector3 box_cast_extents;
    [SerializeField] Animator animator;

    [SerializeField] float dist_from_player;
    [SerializeField] float stun_duration;


    private bool impacted;

    
    void Start()
    {
        transform.position = origin + (facing * dist_from_player);
        transform.rotation = Quaternion.LookRotation(-facing);

        AudioManager.PlayOneShot(activation_sound);
    }


    void Update()
    {
        if (!impacted && AnimationDone())
        {
            Impact();
        }
    }


    bool AnimationDone()
    {
        return (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f && !animator.IsInTransition(0));
    }


    void Impact()
    {
        var hits = Physics.BoxCastAll(box_cast_point.position, box_cast_extents / 2,
            Vector3.down, transform.rotation, 0);

        foreach (var hit in hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == null || character == owner)
                continue;

            character.Damage(damage, owner);
            character.Stun(stun_duration);
        }

        AudioManager.PlayOneShot(hit_sound);
        CameraShake.Shake(0.4f, 0.4f);

        impacted = true;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(box_cast_point.position, box_cast_extents);
    }

}
