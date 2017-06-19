using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMeteorStrike : Projectile
{

    public int max_meteors = 3;
    public float meteor_delay = 0.5f;

    private float timer = 0;
    private float meteor_times = 0;

    private MeteorManager meteor_manager;

    protected override void Start()
    {
        meteor_manager = GameObject.FindObjectOfType<MeteorManager>();
    }


    protected override void Update()
    {
        if (meteor_times < max_meteors)
        {
            timer += Time.deltaTime;

            if (timer >= meteor_delay)
            {
                timer = 0;

                SpawnMeteor();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void SpawnMeteor()
    {
        ++meteor_times;

        meteor_manager.SpawnMeteor();
    }

}
