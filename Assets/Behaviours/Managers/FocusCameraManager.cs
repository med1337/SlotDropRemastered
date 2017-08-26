using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraManager : MonoBehaviour
{
    public enum FocusCameraState
    {
        IDLE,
        FOCUSING,
        LOITERING
    }


    [SerializeField] Camera managed_camera;
    [SerializeField] GameObject scan_plane;
    [SerializeField] LayerMask scan_layer;
    [SerializeField] float step = 5;

    private Vector3 original_position;
    private float original_zoom;

    private Vector3 target_position;
    private float target_zoom;

    public FocusCameraState state;
    private bool focus_complete;
    private float duration;
    private float timer;


    public void Focus(Vector3 _target, float _zoom, float _duration = 3)
    {
        scan_plane.transform.position = new Vector3(0, _target.y, 0);
        Time.timeScale = 0;
        focus_complete = false;

        target_position = _target;
        target_zoom = _zoom;

        state = FocusCameraState.FOCUSING;
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
        switch (state)
        {
            case FocusCameraState.FOCUSING:
            {
                FocusState();
            } break;

            case FocusCameraState.LOITERING:
            {
                LoiterState();
            } break;
        }
    }


    void FocusState()
    {
        if (!FocusStep(target_position, target_zoom))
        {
            state = focus_complete ?
                FocusCameraState.IDLE : FocusCameraState.LOITERING;
        }
    }


    void LoiterState()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= duration)
        {
            state = FocusCameraState.FOCUSING;
            focus_complete = true;
            timer = 0;

            target_position = Vector3.zero;
            target_zoom = original_zoom;
        }
    }


    bool FocusStep(Vector3 _target, float _zoom)
    {
        float zoom_diff = Mathf.Abs(_zoom - managed_camera.orthographicSize);
        float dist_to_target = Vector3.Distance(CalculateRayPosition(), _target);

        if (dist_to_target > 1 && zoom_diff > 0.1f)
        {
            Vector3 target = target_position + original_position;
            target.y = original_position.y;

            ZoomStep(_zoom);
            MoveStep(target);

            return true;
        }
        else
        {
            return false;
        }
    }


    void ZoomStep(float _zoom)
    {
        managed_camera.orthographicSize = Mathf.Lerp(managed_camera.orthographicSize,
            _zoom, step * Time.unscaledDeltaTime);
    }


    void MoveStep(Vector3 _target)
    {
        transform.position = Vector3.Lerp(transform.position,
            _target, step * Time.unscaledDeltaTime);
    }


    Vector3 CalculateRayPosition()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, float.MaxValue, scan_layer);

        return hit.point;
    }


    void OnDrawGizmosSelected()
    {
        if (state == FocusCameraState.IDLE)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, CalculateRayPosition() - transform.position);

        Gizmos.DrawSphere(target_position, 2);
    }

}
