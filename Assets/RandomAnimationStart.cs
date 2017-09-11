using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationStart : MonoBehaviour
{
    public Animator animator;
    public string animation_name;

    void OnEnable()
    {
        animator.Play(animation_name, -1, Random.Range(0.0f,1.0f));
    }
}
