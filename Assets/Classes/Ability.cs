using System.Collections;
using UnityEngine;

public class Ability
{
    public USBCharacter owner;
    public GameObject projectile_prefab;

    private float ready_time;
    

    public void Activate()
    {
        if (!Ready())
            return;

        Projectile projectile = GameObject.Instantiate(projectile_prefab, 
            owner.transform.position, Quaternion.identity).GetComponent<Projectile>();

        projectile.Init(owner);

        ready_time = Time.time + projectile.cooldown;

        // TODO: play projectile activation sound effect.
    }


    bool Ready()
    {
        return Time.time >= ready_time;
    }

}
