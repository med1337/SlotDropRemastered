using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static Camera cam;
    public float zoom_speed = 0.5f;
    private static float target_camera_ortho_size;
    private static float current_camera_ortho_size;
    private const float LERP_TOLERANCE  = 0.05f;
    private float t = 0;
    public static CameraManager instance;


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;
    }
    

    void Start()
    {
        cam = Camera.main;
        current_camera_ortho_size = cam.orthographicSize;
    }


    void Update()
    {
        UpdateOrthographicZoom();
    }


    void UpdateOrthographicZoom()
    {
        if (t >= 1)
            t = 0;

        if (!cam)
            return;

        if (!(Math.Abs(current_camera_ortho_size - target_camera_ortho_size) > LERP_TOLERANCE))//if at value target
            return;//GTFO

        current_camera_ortho_size = Mathf.Lerp(current_camera_ortho_size,
            target_camera_ortho_size, t += Time.deltaTime * zoom_speed);
        cam.orthographicSize = current_camera_ortho_size;    
    }


    public static void SetTargetSize(float _target_camera_ortho_size)
    {
        target_camera_ortho_size = _target_camera_ortho_size;
    }

}
