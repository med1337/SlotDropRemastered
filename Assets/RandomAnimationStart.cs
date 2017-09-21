using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationStart : MonoBehaviour
{
    public Animator animator;
    public string animation_name;
    public float delay_min = 0;
    public float delay_max = 1;

    void OnEnable()
    {
        animator.Play(animation_name, -1, Random.Range(delay_min, delay_max));
    }
}
