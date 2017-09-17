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
    private float panic_chance = 0.005f;
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
        if (_target != null)
        {
            var dist = (_target.position - transform.position);
            dist.y = 0;
            return dist;
        }

        return Vector3.zero;
    }


    public void MoveAI(Vector3 _dir, float _min_magnitude = 0)
    {
        if (_dir.magnitude > _min_magnitude)
            character.Move(_dir.normalized);
        else
            character.Face(_dir.normalized);
    }


    public void CalculateWaypoint()
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


    public void FindClosestOpenSlot()
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


    public bool RollForPanic()
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


    public bool CheckHealthPercentage(float _health_percentage)
    {
        float percent = (float)max_health;
        percent *= 0.01f;

        if (max_health < percent * _health_percentage)
            return true;

        return false;
    }


    public void HandleBasic()
    {
        if (closest_enemy == null && current_basic_delay != 0)
            current_basic_delay = 0;

        if (Time.time < current_basic_delay || closest_enemy == null)
            return;

        current_basic_delay = Time.time + Random.Range(MIN_BASIC_USE, MAX_BASIC_USE);
        character.Attack();
    }


    public void HandleSpecial()
    {
        if (closest_enemy == null && current_special_delay != 0)
            current_special_delay = 0;

        if (Time.time < current_special_delay || closest_enemy == null)
            return;

        current_special_delay = Time.time + Random.Range(MIN_SPECIAL_USE, MAX_SPECIAL_USE);
        character.SlotDrop();
    }

}
