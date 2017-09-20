using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBarrelBomb : Projectile
{
    public GameObject barrel_prefab;
    public float dist_from_player;
    public float y_offset;
    public float effect_radius;
    public float knockback_force;
    public float stun_duration;
    public float throw_force = 10.0f;
    public float fuse_time = 3f;


    void Start()
    {
        GameObject obj = Instantiate(barrel_prefab, origin + new Vector3(0, y_offset, 0), Quaternion.identity);
        obj.transform.position += facing * dist_from_player;
        obj.transform.rotation = Quaternion.LookRotation(-facing);
        obj.transform.Rotate(0, 90, 0);

        Barrel bomb = obj.GetComponent<Barrel>();
        ConfigureBomb(bomb);
        
        bomb.AddForce(facing * (throw_force * 1000));
    }


    void ConfigureBomb(Barrel _bomb)
    {
        _bomb.owner = this.owner;
        _bomb.effect_radius = this.effect_radius;
        _bomb.knockback_force = this.knockback_force;
        _bomb.stun_duration = this.stun_duration;
        _bomb.fuse_time = this.fuse_time;
    }

}
