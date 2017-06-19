using System.Collections;
using UnityEngine;

public class Ability
{
    public USBCharacter owner;
    public GameObject projectile_prefab;

    private float ready_time;
    

    public void Activate()
    {
        if (!IsReady())
            return;

        Projectile projectile = GameObject.Instantiate(projectile_prefab, 
            owner.transform.position, Quaternion.identity).GetComponent<Projectile>();

        projectile.Init(owner);

        ready_time = Time.time + projectile.cooldown;
        GameObject.FindObjectOfType<AudioManager>().PlayOneShot(projectile.activation_sound);
    }


    public bool IsReady()
    {
        return Time.time >= ready_time;
    }

}
