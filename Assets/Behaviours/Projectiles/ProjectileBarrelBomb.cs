using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBarrelBomb : Projectile
{
    public GameObject barrel_prefab;
    public float effect_radius;
    public float knockback_force;
    public float stun_duration;
    public float start_offset_y = 5.0f;
    public int max_bombs = 3;
    public float throw_force = 10.0f;

    [Range(1000, 4000)]
    public float min_throw_range = 2000;

    [Range(1000, 4000)]
    public float max_throw_range = 3000;

    private List<Vector3> prev_drop_vectors = new List<Vector3>();


    void Start()
    {
        // Spawn bombs!
        for (int i = 0; i < max_bombs; ++i)
        {
            GameObject obj = Instantiate(barrel_prefab, owner.transform.position +
                new Vector3(0, start_offset_y, 0), Quaternion.identity);

            Barrel bomb = obj.GetComponent<Barrel>();

            ConfigureBomb(bomb);
            GenerateDropVector(bomb);
        }

        Destroy(gameObject);
    }


    void ConfigureBomb(Barrel _bomb)
    {
        _bomb.owner = this.owner;
        _bomb.effect_radius = this.effect_radius;
        _bomb.knockback_force = this.knockback_force;
        _bomb.stun_duration = this.stun_duration;
    }


    void GenerateDropVector(Barrel _bomb)
    {
        Vector3 drop_vector = Vector3.zero;

        bool vector_valid = false;

        do
        {
            float random_x = Random.Range(-max_throw_range, max_throw_range);
            random_x = Mathf.Clamp(random_x, -min_throw_range, max_throw_range);

            float random_z = Random.Range(-max_throw_range, max_throw_range);
            random_z = Mathf.Clamp(random_z, -min_throw_range, max_throw_range);

            drop_vector = new Vector3(random_x, throw_force * 1000, random_z);

            int test_passes = 0;
            foreach (Vector3 v in prev_drop_vectors)
            {
                if (Vector3.Distance(drop_vector, v) >= min_throw_range)
                    ++test_passes;
            }

            if (test_passes >= prev_drop_vectors.Count)
                vector_valid = true;

        } while (!vector_valid);

        _bomb.AddForce(drop_vector);

        prev_drop_vectors.Add(drop_vector);
    }

}
