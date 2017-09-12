using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform eye;
    [SerializeField] LineRenderer line;
    [SerializeField] AudioClip charge_sound;
    [SerializeField] AudioClip fire_sound;
    [SerializeField] GameObject face_indicator;

    public USBCharacter owner;
    public float lifetime;
    public int damage;
    public float scan_radius;
    public float laser_radius;
    public float grow_speed = 10;

    private Vector3 original_scale;
    private Vector3 target_scale = Vector3.one;
    private float timer;
    private float lifetime_divisor = 5;


    void Start()
    {
        original_scale = transform.localScale;
    }

    
    void Update()
    {
        float prev_timer = timer;
        timer += Time.deltaTime;

        if (timer >= lifetime / lifetime_divisor &&
            prev_timer < lifetime / lifetime_divisor)
        {
            StartCoroutine(FireLaser());
        }

        HandleScale();
    }


    IEnumerator FireLaser()
    {
        var hits = Physics.SphereCastAll(transform.position, scan_radius,
            Vector3.down, 0, 1 << LayerMask.NameToLayer("Player")).ToList();

        if (owner != null)
            hits.RemoveAll(elem => elem.collider.transform.GetInstanceID() == owner.transform.GetInstanceID());

        if (hits.Count == 0)
        {
            ScheduleDeletion();
        }
        else
        {
            Vector3 closest_target = CalculateClosestTarget(hits);
            Vector3 direction = (closest_target - eye.transform.position).normalized;

            UpdateFaceIndicator(direction);
            AudioManager.PlayOneShot(charge_sound);

            yield return new WaitForSeconds(0.5f);

            float distance;
            Vector3 hit_point = CalculateHitPoint(direction, out distance);

            line.startWidth = laser_radius * 0.33f;
            line.positionCount = 2;
            line.SetPosition(0, eye.transform.position);
            line.SetPosition(1, hit_point);

            AudioManager.PlayOneShot(fire_sound);
            CauseDamage(direction, distance);

            yield return new WaitForSeconds(lifetime * 0.2f);

            face_indicator.SetActive(false);
            line.positionCount = 0;

            yield return new WaitForSeconds(lifetime * 0.2f);

            ScheduleDeletion();
        }
    }


    Vector3 CalculateClosestTarget(List<RaycastHit> _hits)
    {
        Vector3 closest_position = eye.transform.position;
        float closest_dist = float.MaxValue;

        foreach (var hit in _hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            float dist = (character.body_group.transform.position - transform.position).sqrMagnitude;
            if (dist >= closest_dist)
                continue;

            closest_dist = dist;
            closest_position = hit.transform.position;
        }

        closest_position.y = eye.position.y;

        return closest_position;
    }


    void UpdateFaceIndicator(Vector3 _direction)
    {
        face_indicator.SetActive(true);

        Vector3 look_at = face_indicator.transform.position + (_direction * 3);
        face_indicator.transform.LookAt(look_at);
        face_indicator.transform.Rotate(new Vector3(90, 0, 0));
    }


    Vector3 CalculateHitPoint(Vector3 _direction, out float _distance)
    {
        int layer_mask = 1 << LayerMask.NameToLayer("Default") |
                         1 << LayerMask.NameToLayer("Prop") |
                         1 << LayerMask.NameToLayer("PC");

        RaycastHit hit;
        Physics.Raycast(eye.transform.position, _direction, out hit, Mathf.Infinity, layer_mask);

        if (hit.point == Vector3.zero)
        {
            hit.distance = 1000;
            hit.point = _direction * hit.distance;
        }

        _distance = hit.distance;
        return hit.point;
    }


    void CauseDamage(Vector3 _direction, float _distance)
    {
        var laser_hits = Physics.SphereCastAll(eye.position, 1, _direction,
            _distance, 1 << LayerMask.NameToLayer("Player"));

        foreach (var hit in laser_hits)
        {
            USBCharacter character = hit.collider.GetComponent<USBCharacter>();

            if (character == owner)
                continue;

            character.Damage(damage, eye.position, owner);
        }
    }


    void ScheduleDeletion()
    {
        target_scale = original_scale;
        Destroy(this.gameObject, lifetime / grow_speed);
    }


    void HandleScale()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, target_scale, grow_speed * Time.deltaTime);
    }

}
