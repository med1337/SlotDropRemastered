using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform cam_transform;
    public Vector3 original_local_pos;

    private static CameraShake instance = null;

    private bool shaking = false;
    private bool shaking_paused = false;
    private float shake_strength = 0.7f;
    private float shake_duration = 0.0f;
    private float shake_time = 0.0f;

    [SerializeField] private AnimationCurve shakeDecayRate = null;


    public static void Shake(float _strength, float _duration)
    {
        instance.StartShake(_strength, _duration);
    }


    public static void Pause()
    {
        instance.shaking_paused = true;
    }


    public static void Resume()
    {
        instance.shaking_paused = false;
    }


    void StartShake(float _strength, float _duration)
    {
        shake_time = 0.0f;

        shake_strength = _strength;
        shake_duration = _duration;

        if (!shaking)
        {
            original_local_pos = cam_transform.localPosition;
            shaking = true;
        }
    }


    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void Update()
    {
        if (shaking_paused)
            return;

        if (shaking)
        {
            shake_time += Time.deltaTime;

            HandleShake();
        }
    }


    void HandleShake()
    {
        if (shake_time < shake_duration)
        {
            cam_transform.localPosition = original_local_pos +
                ((Random.insideUnitSphere * shake_strength) *
                shakeDecayRate.Evaluate(shake_time / shake_duration));
        }
        else
        {
            shaking = false;
            cam_transform.localPosition = original_local_pos;
        }
    }

}
