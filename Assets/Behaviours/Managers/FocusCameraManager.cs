using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraManager : MonoBehaviour
{
    [SerializeField] Camera managed_camera;
    [SerializeField] GameObject scan_plane;
    [SerializeField] LayerMask scan_layer;

    private Vector3 original_position;
    private float original_zoom;

    private Vector3 target_position;
    private float target_zoom;

    private bool focusing;
    private float duration;
    private float timer;


    public void Focus(Vector3 _target, float _zoom, float _duration = 3)
    {
        scan_plane.transform.position = new Vector3(0, _target.y, 0);
        Time.timeScale = 0;

        target_position = _target;
        target_zoom = _zoom;

        focusing = true;
        duration = _duration;
        timer = 0;
    }


    void Start()
    {
        original_position = this.transform.position;
        original_zoom = managed_camera.orthographicSize;
    }


    void Update()
    {
        if (!focusing)
            return;

        if (Vector3.Distance(CalculateRayPosition(), target_position) > 5)
        {
            Vector3 target = target_position + original_position;
            target.y = original_position.y;

            transform.position = Vector3.Lerp(transform.position, target, 2 * Time.unscaledDeltaTime);
        }
        else
        {
            Deactivate();
        }
    }


    Vector3 CalculateRayPosition()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, float.MaxValue, scan_layer);

        return hit.point;
    }


    void Deactivate()
    {
        scan_plane.transform.position = Vector3.zero;
        Time.timeScale = 1;

        this.transform.position = original_position;
        managed_camera.orthographicSize = original_zoom;

        target_position = original_position;
        target_zoom = original_zoom;

        focusing = false;
        duration = 0;
        timer = 0;
    }


    void OnDrawGizmosSelected()
    {
        if (!focusing)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, CalculateRayPosition() - transform.position);
    }


}
