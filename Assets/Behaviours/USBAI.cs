using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

enum AIState : uint
{
    Wandering,
    Attacking,
    Retreating,
    SeekSlot
}


public class USBAI : MonoBehaviour
{
    [HideInInspector] public USBCharacter character;
    [HideInInspector] public USBCharacter closest_enemy;
    [HideInInspector] public USBSlot closest_slot;

    [SerializeField] private StateMachine ai_state_machine;
    private float waypoint_timer = 0;

    private const float MIN_BASIC_USE = 1;
    private const float MAX_BASIC_USE = 2;
    private const float MIN_SPECIAL_USE = 2;
    private const float MAX_SPECIAL_USE = 4;

    private float current_basic_delay;
    private float current_special_delay;
    private float panic_chance = 0.0005f;
    private float panic_low_health_mod = 1.1f;
    private int max_health = 0;


    private void OnDestroy()
    {
        if (ai_state_machine != null)
            Destroy(ai_state_machine);
    }


    void Awake()
    {
        character = GetComponent<USBCharacter>();

        if (character == null)
        {
            Destroy(this);
        }
        else
        {
            character.AddSpeedModifier(0.75f, float.MaxValue);
            max_health = character.health;
        }
    }


    public void SetAIBehaviour(StateMachine _behaviour)
    {
        ai_state_machine = Object.Instantiate(_behaviour);//create own copy of the state machine asset
        ai_state_machine.InitStateMachine(this);//intitialise
    }


    void Update()
    {
        if (ai_state_machine == null)
        {
            Debug.LogWarning("AI did not recieve state machine behaviour!");
            return;
        }

        ai_state_machine.UpdateStateMachine();
    }


    public Vector3 CalculateMoveVector(Transform _target)
    {
        if (_target == null)
            return Vector3.zero;

        var dist = (_target.position - transform.position);
        dist.y = 0;
        return dist;
    }


    public void MoveAI(Vector3 _dir, float _min_magnitude = 0)
    {
        if (_dir.magnitude > _min_magnitude)
            character.Move(_dir.normalized);
        else
            character.Face(_dir.normalized);
    }


    public void FindClosestEnemy()
    {
        waypoint_timer += Time.deltaTime;

        if (!(waypoint_timer >= 0.2f))
            return;

        waypoint_timer = 0;
        var enemies = GameManager.scene.respawn_manager.alive_characters;
        float closest_distance = float.PositiveInfinity;

        foreach (var enemy in enemies)
        {
            if (enemy == character)
                continue;

            if (enemy.is_titan)
            {
                closest_enemy = enemy;
                return;
            }

            float dist = (enemy.transform.position - transform.position).sqrMagnitude;
            if (dist >= closest_distance)
                continue;

            closest_distance = dist;
            closest_enemy = enemy;
        }
    }


    public bool FindClosestOpenSlot()
    {
        var slot_list = GameManager.scene.slot_manager.slots;
        float closest_distance = float.PositiveInfinity;
        closest_slot = null;

        foreach (var slot in slot_list)
        {
            if (slot.golden_slot)
            {
                if (character.is_titan)
                { 
                    closest_slot = slot;
                    return true;
                }
                continue;
            }

            float dist = (slot.transform.position - transform.position).sqrMagnitude;
            if (dist >= closest_distance)
                continue;

            if (slot.slottable)
            {
                closest_distance = dist;
                closest_slot = slot;
            }
        }

        return closest_slot != null;
    }


    public bool CheckHealthPercentage(float _health_percentage)
    {
        float percent = (float)max_health;
        percent *= 0.01f;
        return max_health <= (percent * _health_percentage);
    }


    public void HandleBasic()
    {
        if (closest_enemy == null && current_basic_delay != 0)
            current_basic_delay = 0;

        if (Time.time < current_basic_delay || closest_enemy == null)
            return;
 
        FaceClosestEnemy();
        current_basic_delay = Time.time + Random.Range(MIN_BASIC_USE, MAX_BASIC_USE);
        character.Attack();
    }


    public void HandleSpecial()
    {
        if (closest_enemy == null && current_special_delay != 0)
            current_special_delay = 0;

        if (Time.time < current_special_delay || closest_enemy == null)
            return;
        
        FaceClosestEnemy();
        current_special_delay = Time.time + Random.Range(MIN_SPECIAL_USE, MAX_SPECIAL_USE);
        character.SlotDrop();
    }


    private void FaceClosestEnemy()
    {
        if (closest_enemy == null)
            return;

        Vector3 dir = CalculateMoveVector(closest_enemy.transform);
        dir.Normalize();
        character.Face(dir);
    }


    public float DistanceFromCharacter(Transform _target)
    {
        return _target == null ? 0 : (_target.position - transform.position).sqrMagnitude;
    }
}
