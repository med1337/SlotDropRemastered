using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class HeightIndicator : MonoBehaviour
{
    [SerializeField] private float surface_offset = 0.1f;
    [SerializeField] private float distance_min = 1f;
    [SerializeField] private GameObject indicator;
    [SerializeField] private LayerMask hit_layer;

    private LineRenderer line;
    private Collider parent_collider;
    private Vector3 ray_offset = new Vector3(0, 1.0f, 0);


    void Start()
    {
        parent_collider = GetComponent<Collider>();
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
        line.enabled = false;
        indicator.SetActive(false);
    }


    void Update()
    {
        if (indicator == null)
            return;

        RayCastFloor();
    }
    

    void RayCastFloor()
    {
        RaycastHit hit;
        bool floor_hit = Physics.Raycast(transform.position + ray_offset, -Vector3.up, out hit, float.MaxValue, hit_layer);
   
        if (indicator && floor_hit && hit.distance >= distance_min)
        {
            indicator.SetActive(true);

            Vector3 floor_pos = new Vector3(transform.position.x, hit.point.y + surface_offset, transform.position.z);
                
            indicator.transform.position = floor_pos;
            line.enabled = true;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, floor_pos);

            return;
        } 

        indicator.SetActive(false);
        line.enabled = false;
    }

}