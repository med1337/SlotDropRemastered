using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Vector3 original_local_pos = Vector3.zero;

    private static CameraShake instance;
    private Transform cam_transform = null;

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
            cam_transform = Camera.main.transform;
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

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(CameraShake))]
public class CameraShakeEditor : UnityEditor.Editor
{
    private float strength = 0.2f;
    private float duration = 0.2f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnityEditor.EditorGUILayout.Space();
        UnityEditor.EditorGUILayout.LabelField("Camera Shake Test", UnityEditor.EditorStyles.boldLabel);

        CameraShake myScript = (CameraShake)target;

        strength = UnityEditor.EditorGUILayout.Slider("Strength", strength, 0.1f, 5.0f);
        duration = UnityEditor.EditorGUILayout.Slider("Duration", duration, 0.1f, 10.0f);

        if (GUILayout.Button("Shake"))
        {
            CameraShake.Shake(strength, duration);
        }
    }
}
#endif
