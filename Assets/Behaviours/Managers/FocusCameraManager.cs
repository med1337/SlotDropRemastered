using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCameraManager : MonoBehaviour
{
    private enum FocusCameraState
    {
        IDLE,
        FOCUSING,
        LOITERING
    }

    [SerializeField] Camera managed_camera;
    [SerializeField] GameObject scan_plane;
    [SerializeField] LayerMask scan_layer;
    [SerializeField] float step = 5;

    private Vector3 last_idle_position;

    private Vector3 target_position;
    private float target_zoom;

    private FocusCameraState state;
    private bool focus_complete;
    private bool affected_timescale;
    private float duration;
    private float idle_timer;
    private float loiter_timer;

    // Debug.
    private Vector3 raw_target;


    public void Focus(Vector3 _target, float _zoom, float _duration = 1.5f, bool _affect_timescale = true)
    {
        if (_affect_timescale)
            Time.timeScale = 0;

        affected_timescale = _affect_timescale;
        focus_complete = false;

        raw_target = _target;
        target_position = CalculateRayToScanPlane(_target);
        target_zoom = _zoom;

        state = FocusCameraState.FOCUSING;
        duration = _duration;
        loiter_timer = 0;
    }


    void Start()
    {
        last_idle_position = transform.position;
    }


    void Update()
    {
        switch (state)
        {
            case FocusCameraState.IDLE:
            {
                IdleState();
            }
                break;

            case FocusCameraState.FOCUSING:
            {
                FocusState();
            }
                break;

            case FocusCameraState.LOITERING:
            {
                LoiterState();
            }
                break;
        }
    }


    void IdleState()
    {
        idle_timer += Time.deltaTime;

        if (idle_timer >= 0.1f)
        {
            idle_timer = 0;

            Vector3 avg_position = new Vector3();
            foreach (USBCharacter character in GameManager.scene.respawn_manager.alive_characters)
                avg_position += character.transform.position;

            for (int i = 0; i < GameManager.scene.respawn_manager.alive_characters.Count / 2; i++)
            {
                avg_position += GameManager.scene.pc_manager.transform.position;
            }
            avg_position /= GameManager.scene.respawn_manager.alive_characters.Count + 2;

            last_idle_position = CalculateRayToScanPlane(avg_position);
        }

        FocusStep(last_idle_position, 30);
    }


    void FocusState()
    {
        if (!FocusStep(target_position, target_zoom))
        {
            if (focus_complete)
            {
                state = FocusCameraState.IDLE;

                if (affected_timescale && Time.timeScale == 0)
                    Time.timeScale = 1;
            }
            else
            {
                state = FocusCameraState.LOITERING;
            }
        }
    }


    void LoiterState()
    {
        loiter_timer += Time.unscaledDeltaTime;

        if (loiter_timer >= duration)
        {
            state = FocusCameraState.FOCUSING;
            focus_complete = true;
            loiter_timer = 0;
        }
    }


    bool FocusStep(Vector3 _target, float _zoom)
    {
        float zoom_diff = Mathf.Abs(_zoom - managed_camera.orthographicSize);
        float dist_to_target = (_target - transform.position).sqrMagnitude;

        if (dist_to_target > 0.1f || zoom_diff > 0.1f)
        {
            ZoomStep(_zoom);
            MoveStep(_target);

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


    Vector3 CalculateRayToScanPlane(Vector3 _shoot_point)
    {
        Ray ray = new Ray(_shoot_point, -transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, float.MaxValue, scan_layer);

        return hit.point;
    }


    void OnDrawGizmosSelected()
    {
        if (state == FocusCameraState.IDLE)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawRay(raw_target, target_position - raw_target);
        Gizmos.DrawRay(transform.position, transform.forward * 100);

        Gizmos.DrawSphere(raw_target, 2);
        Gizmos.DrawSphere(target_position, 2);
    }
}