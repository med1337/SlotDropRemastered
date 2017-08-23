using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorManager : MonoBehaviour
{
    public Meteor meteor_prefab;
    public SpawnAreaCircle meteor_spawn_area;
    public float spawn_delay;

    private float duration_timer;
    private float duration;
    private float spawn_timer;

    
    public void SpawnMeteors(float _duration)
    {
        Init();

        duration = _duration;
        this.enabled = true;
    }


    void OnDisable()
    {
        Init();
    }

    
    void Init()
    {
        duration = 0;
        duration_timer = 0;
        spawn_timer = 0;
    }


    void Update()
    {
        if (duration_timer < duration)
        {
            duration_timer += Time.deltaTime;
            spawn_timer += Time.deltaTime;

            if (spawn_timer >= spawn_delay)
            {
                spawn_timer = 0;
                SpawnMeteor();
            }
        }
        else
        {
            this.enabled = false;
        }
    }


    void SpawnMeteor()
    {
        GameObject meteor_clone = Instantiate(meteor_prefab).gameObject;
        Vector2 random_circle_location = Random.insideUnitCircle * meteor_spawn_area.spawn_radius;

        AudioManager.PlayOneShot("meteor_rain_start");

        meteor_clone.transform.position = new Vector3(meteor_spawn_area.transform.position.x + random_circle_location.x,
            meteor_spawn_area.transform.position.y,
            meteor_spawn_area.transform.position.z + random_circle_location.y); // spawn meteor at random position
    }
}