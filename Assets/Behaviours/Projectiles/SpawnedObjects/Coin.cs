using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] FadableGraphic fade;
    [SerializeField] TransformFollower follower;
    [SerializeField] GameObject sprite_child;
    [SerializeField] float rise_speed;
    [SerializeField] float lifetime;


    public void SetFollowTarget(Transform _target, Vector3 _offset)
    {
        follower.target = _target;
        follower.offset = _offset;
    }


    void Start()
    {
        fade.FadeOut(lifetime);
        Destroy(this.gameObject, lifetime);
    }


    void Update()
    {
        sprite_child.transform.localPosition += new Vector3(0, rise_speed * Time.deltaTime);
    }

}
