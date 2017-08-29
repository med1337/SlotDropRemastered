using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTitanFire : Projectile
{
    public GameObject fire_block;

    public float blast_delay;
    public float blast_spacing;
    public int max_blasts;

    public float damage_delay;

    private float timer;
    private float blast_times;


	void Start()
    {
        timer = blast_delay;
    }


    void Update()
    {
        if (blast_times < max_blasts)
        {
            timer += Time.deltaTime;

            if (timer >= blast_delay)
            {
                timer = 0;

                CreateBlast();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void CreateBlast()
    {
        AudioManager.PlayOneShot("titan_fire");

        ++blast_times;
        Vector3 next_pos = origin + (facing * blast_spacing) * blast_times;

        var hits = Physics.RaycastAll(next_pos + new Vector3(0, 5, 0), -Vector3.up, LayerMask.NameToLayer("Floor"));

        if (hits.Length > 0)
        {
            var clone = Instantiate(fire_block, next_pos, Quaternion.LookRotation(facing));
            clone.AddComponent<TempParticle>();

            clone.GetComponent<FireBlock>().Init(owner, damage, damage_delay, Vector3.one * 2.5f, facing);
            CameraShake.Shake(0.3f, 0.3f);
        }

    }

}
