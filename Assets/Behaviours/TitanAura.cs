using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanAura : MonoBehaviour
{
    public GameObject sparkle_prefab;
    public float throw_force = 10.0f;

    public float min_throw_delay = 0.2f;
    public float max_throw_delay = 1.2f;

    [Range(1000, 4000)]
    public float min_throw_range = 2000;

    [Range(1000, 4000)]
    public float max_throw_range = 3000;

    private bool sparkle_queued;
    private USBCharacter owner;


    public void Init(USBCharacter _owner)
    {
        owner = _owner;
    }


    void Update()
    {
        if (sparkle_queued)
            return;

        sparkle_queued = true;

        Invoke("LaunchSparkle", Random.Range(min_throw_delay, max_throw_delay));
    }


    void LaunchSparkle()
    {
        GameObject clone = Instantiate(sparkle_prefab, transform.position + new Vector3(0, 10, 0), Quaternion.identity);
        Sparkle sparkle = clone.GetComponent<Sparkle>();

        Vector3 drop_vector = Vector3.zero;

        float random_x = Random.Range(-max_throw_range, max_throw_range);
        random_x = Mathf.Clamp(random_x, -min_throw_range, max_throw_range);

        float random_z = Random.Range(-max_throw_range, max_throw_range);
        random_z = Mathf.Clamp(random_z, -min_throw_range, max_throw_range);

        drop_vector = new Vector3(random_x, throw_force * 1000, random_z);
        sparkle.Init(owner, drop_vector);

        sparkle_queued = false;
    }

}
