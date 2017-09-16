using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AIState
{
    Idle,
    Attacking,
    Retreating,
    SeekSlot
}


public class USBAI : MonoBehaviour
{
    private USBCharacter character;
    [SerializeField] private AIState current_state = AIState.Idle;
    private float waypoint_timer = 0;
    private USBCharacter closest_enemy;
    private USBSlot closest_slot;

    private const float MIN_BASIC_USE = 1;
    private const float MAX_BASIC_USE = 2;
    private const float MIN_SPECIAL_USE = 2;
    private const float MAX_SPECIAL_USE = 4;

    private float current_basic_delay;
    private float current_special_delay;
    private float panic_chance = 0.005f;
    private float panic_low_health_mod = 1.1f;
    private float panic_timer;
    private float panic_duration = 0.6f;
    private int max_health = 0;
    float max_distance_to_target = 5;


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


    void Update()
    {
        UpdateAIState();
    }


    void UpdateAIState()
    {
        CalculateWaypoint();

        if (character.stats.score >= 100)
            current_state = AIState.SeekSlot;

        if (character.loadout_name == "Gold")
        {
            current_state = AIState.Attacking;
            if (character.stats.score >= 300)
                current_state = AIState.SeekSlot;

        }

        switch (current_state)
        {
            case AIState.Idle: StateIdle(); //Debug.Log(character.loadout_name + " idle");
                break;
            case AIState.Attacking: StateAttacking(); //Debug.Log(character.loadout_name + " attacking");
                break;
            case AIState.Retreating: StateRetreating(); //Debug.Log(character.loadout_name + " fleeing");
                break;
            case AIState.SeekSlot: StateSeekSlot(); //Debug.Log(character.loadout_name + " seeking slot");
                break;
        }
    }


    void StateIdle()
    {
        if (CheckTransitionToRetreating())
            return;

        if (CheckTransitionToAttacking())
            return;

        if (CheckHealthPercentage(30) &&
            Vector3.Distance(closest_enemy.transform.position, character.transform.position) > 3)
        {
            current_state = AIState.SeekSlot;
            return;
        }

        //TODO: Implement wander state. 
    }


    void StateAttacking()
    {
        if (CheckTransitionToIdle())
            return;

        if (CheckTransitionToRetreating())
            return;

        if (CheckHealthPercentage(30))
            current_state = AIState.SeekSlot;

        ChaseMovement();
        HandleBasic();
        HandleSpecial();
    }


    void StateRetreating()
    {
        if (CheckPanicEnd())
            return;

        FleeMovement();
        panic_timer -= Time.deltaTime;
    }


    void StateSeekSlot()
    {
        FindClosestOpenSlot();

        SeekSlotMovement();

        if (SeekSlotMovement())
            current_state = AIState.Idle;
    }


    bool CheckTransitionToIdle()
    {
        if (closest_enemy == null)
        {
            current_state = AIState.Idle;
            return true;
        }

        return false;
    }


    bool CheckTransitionToAttacking()
    {
        if (closest_enemy != null && !CheckHealthPercentage(30))
        {
            current_state = AIState.Attacking;
            return true;
        }

        return false;
    }


    bool CheckTransitionToRetreating()
    {
        if (RollForPanic())
        {
            current_state = AIState.Retreating;
            panic_timer = panic_duration; //start panic
            return true;
        }

        return false;
    }


    bool CheckPanicEnd()
    {
        if (panic_timer <= 0)
        {
            panic_timer = panic_duration;
            current_state = AIState.Idle;
            return true;
        }

        return false;
    }


    void ChaseMovement()
    {
        if (closest_enemy == null)
            return;

        CalculateWaypoint();
        
        if (Vector3.Distance(closest_enemy.transform.position, character.transform.position) >
            max_distance_to_target)
            MoveAI(CalculateMoveVector(closest_enemy.transform), 3);
    }


    void FleeMovement()
    {
        if (closest_enemy == null)
            return;

        CalculateWaypoint();
        Vector3 dir = CalculateMoveVector(closest_enemy.transform);
        dir *= -1;
        MoveAI(dir, 3);
    }


    bool SeekSlotMovement()
    {
        if (closest_slot == null)
            return false;

        MoveAI(CalculateMoveVector(closest_slot.transform), 0.5f);

        if (Vector3.Distance(closest_slot.transform.position, character.transform.position) < 0.6f)
        {
            if (closest_slot.slottable)
            {
                character.SlotDrop();
                return true;
            }

            FindClosestOpenSlot();
            return false;
        }

        return false;
    }


    Vector3 CalculateMoveVector(Transform _target)
    {
        if (_target != null)
        {
            var dist = (_target.position - transform.position);
            dist.y = 0;
            return dist;
        }

        return Vector3.zero;
    }


    void MoveAI(Vector3 _dir, float min_magnitude = 0)
    {
        if (_dir.magnitude > min_magnitude)
            character.Move(_dir.normalized);
        else
            character.Face(_dir.normalized);
    }


    void CalculateWaypoint()
    {
        waypoint_timer += Time.deltaTime;

        if (waypoint_timer >= 0.2f)
        {
            waypoint_timer = 0;
            var enemies = GameManager.scene.respawn_manager.alive_characters;
            float closest_distance = float.PositiveInfinity;

            foreach (var enemy in enemies)
            {
                if (enemy == character)
                    continue;

                float dist = (enemy.transform.position - transform.position).sqrMagnitude;
                if (dist >= closest_distance)
                    continue;

                closest_distance = dist;
                closest_enemy = enemy;
            }
        }
    }


    void FindClosestOpenSlot()
    {
        var slot_list = GameManager.scene.slot_manager.slots;
        float closest_distance = float.PositiveInfinity;

        foreach (var slot in slot_list)
        {
            if (slot.golden_slot)
            {
                if (character.loadout_name == "Gold")
                { 
                    closest_slot = slot;
                    return;
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
    }


    bool RollForPanic()
    {
        if (closest_enemy == null)
            return false;

        if (Random.Range(0, 100) < panic_chance + panic_low_health_mod && CheckHealthPercentage(30))//roll for panic
            return true;

        if (CheckHealthPercentage(10))
            return true;

        if (Random.Range(0, 100) < panic_chance)
            return true;

        return false;//no panic
    }


    bool CheckHealthPercentage(float percentage_chance)
    {
        float percent = (float)max_health;
        percent *= 0.01f;

        if (max_health < percent * percentage_chance)
            return true;

        return false;
    }


    void HandleBasic()
    {
        if (closest_enemy == null && current_basic_delay != 0)
            current_basic_delay = 0;

        if (Time.time < current_basic_delay || closest_enemy == null)
            return;

        current_basic_delay = Time.time + Random.Range(MIN_BASIC_USE, MAX_BASIC_USE);
        character.Attack();
    }


    void HandleSpecial()
    {
        if (closest_enemy == null && current_special_delay != 0)
            current_special_delay = 0;

        if (Time.time < current_special_delay || closest_enemy == null)
            return;

        current_special_delay = Time.time + Random.Range(MIN_SPECIAL_USE, MAX_SPECIAL_USE);

        character.SlotDrop();
    }

}
