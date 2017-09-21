using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScale : MonoBehaviour
{
    [SerializeField] private float min_scale = 1;
    [SerializeField] private float max_scale = 1;

    private void OnEnable()
    {
        float scale = Random.Range(min_scale, max_scale);
        transform.localScale = new Vector3(scale, scale, scale);
    }


}
