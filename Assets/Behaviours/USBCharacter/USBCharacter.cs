using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    private enum EnergyState
    {
        DRAINING,
        INCREASING
    }


    public int target_score
    {
        get { return target_score_; }
        set { target_score_ = value; }
    }

    public float target_energy
    {
        get { return target_energy_; }
        set
        {
            if (value > target_energy_)
                energy_state = EnergyState.INCREASING;

            target_energy_ = Mathf.Clamp(value, 0, 100);
        }
    }

    public int score { get; private set; }
    public float energy { get; private set; }

    private float energy_lerp_speed;
    private int score_lerp_speed;
    private float energy_drain_rate;

    private USBCharacter owner;
    private PlayerHUD hud;
    private EnergyState energy_state;

    private int target_score_;
    private float target_energy_;


    public CharacterStats(USBCharacter _owner, PlayerHUD _hud,
        ref float _energy_lerp_speed, ref int _score_lerp_speed, ref float _energy_drain_rate)
    {
        owner = _owner;
        hud = _hud;
        energy_lerp_speed = _energy_lerp_speed;
        score_lerp_speed = _score_lerp_speed;
        energy_drain_rate = _energy_drain_rate;
    }


    public void Update()
    {
        UpdateScore();
        UpdateEnergy();
    }


    void UpdateScore()
    {
        if (score == target_score)
            return;

        switch (energy_state)
        {
            case EnergyState.DRAINING:
            {
                if (energy >= target_energy && score > target_score)
                    score = target_score;

                int adjustment = Random.Range(0, score_lerp_speed - 1);
                score += target_score > score ? adjustment : -adjustment;

                if (Mathf.Abs(target_score - score) <= adjustment)
                    score = target_score;
            }
                break;

            case EnergyState.INCREASING:
            {
                int adjustment = Random.Range(0, score_lerp_speed - 1);
                score += target_score > score ? adjustment : -adjustment;

                if (score < target_score)
                    score = target_score;
            }
                break;
        }

        hud.UpdateScore(score);
    }


    void UpdateEnergy()
    {
        float prev_energy = energy;

        switch (energy_state)
        {
            case EnergyState.DRAINING:
            {
                target_energy -= energy_drain_rate * Time.deltaTime;

                if (energy > target_energy)
                    energy = target_energy;
            }
                break;

            case EnergyState.INCREASING:
            {
                energy += target_energy > energy ? energy_lerp_speed : -energy_lerp_speed;
                energy = Mathf.Clamp(energy, 0, 100);

                if (Mathf.Abs(target_energy - energy) <= energy_lerp_speed)
                    energy = target_energy;

                if (energy == target_energy)
                    energy_state = EnergyState.DRAINING;
            }
                break;
        }

        if (prev_energy > 0 && energy <= 0)
            owner.gameObject.SendMessage("PowerDown");
        else if (!owner.is_titan && (prev_energy < 100 && energy >= 100))
            owner.gameObject.SendMessage("BecomeTitan");

        hud.UpdateEnergy(energy);
    }
}


public class USBCharacter : MonoBehaviour
{
    public CharacterStats stats { get; private set; }
    public Vector3 last_facing { get; private set; }

    public string loadout_name
    {
        get { return loadout.name; }
    }

    public Vector3 move_dir { get; private set; }
    public bool is_titan { get; private set; }
    public bool controls_disabled { get; private set; }

    [Header("Parameters")] [SerializeField] int score_on_kill = 50;
    [SerializeField] int heal_on_kill = 20;
    public int energy_on_slot = 25;
    [SerializeField] float energy_drain_rate = 1.25f;
    public float energy_score_factor = 0.1f;
    [SerializeField] float energy_lerp_speed = 0.5f;
    [SerializeField] int score_lerp_speed = 5;
    [SerializeField] float velocity_limit = 20;

    [Header("References")] public GameObject body_group;
    public Rigidbody rigid_body;

    [SerializeField] Projector shadow;
    [SerializeField] Animator animator;
    [SerializeField] GameObject face_indicator;
    [SerializeField] Renderer body_renderer;
    [SerializeField] SpriteRenderer hat_renderer;
    [SerializeField] GameObject stun_effect;
    [SerializeField] PlayerHUD hud;
    [SerializeField] FadableGraphic damage_flash;
    [SerializeField] ShakeModule shake_module;
    [SerializeField] GameObject hit_particle;
    [SerializeField] GameObject heal_particle;
    [SerializeField] Transform slot_tracker;

    private MovementCalculator movement_calculator = new MovementCalculator();
    private USBLoadout loadout = new USBLoadout();
    private int health;
    private float original_mass;

    private Ability basic_ability = new Ability();
    private Ability special_ability = new Ability();

    private bool moving;
    private bool slot_dropping;
    private bool face_locked;
    private USBSlot last_slot_hit;
    private int score_;
    private TitanAura titan_aura;

    private int shown_score;
    private int shown_energy;


    public void Move(Vector3 _dir)
    {
        if (_dir != Vector3.zero)
        {
            if (!face_locked)
                last_facing = _dir.normalized;

            moving = true;
        }

        if (!controls_disabled)
            move_dir = _dir;

        UpdateFaceIndicator();
    }


    public void Face(Vector3 _dir)
    {
        if (_dir != Vector3.zero)
        {
            if (!face_locked)
                last_facing = _dir.normalized;
        }

        UpdateFaceIndicator();
    }


    public void Attack()
    {
        if (slot_dropping || controls_disabled || Time.timeScale == 0)
            return;

        basic_ability.Activate();
    }


    public void SlotDrop()
    {
        if (slot_dropping || controls_disabled || Time.timeScale == 0)
            return;

        if (special_ability.IsReady())
        {
            slot_dropping = true;
            animator.SetTrigger("slot_drop");

            ScanForSlot();
        }
    }


    public void SetFaceLocked(bool _locked)
    {
        face_locked = _locked;
    }


    public void SetColour(Color _color)
    {
        body_renderer.material.color = _color;
        face_indicator.GetComponent<SpriteRenderer>().material.color = _color;
    }


    public void AddSpeedModifier(float _modifier, float _duration)
    {
        movement_calculator.AddSpeedModifier(_modifier, _duration);
    }


    public void AssignLoadout(USBLoadout _loadout, bool _heal = true)
    {
        // TODO: remove previous particle effect ..

        loadout = _loadout;

        if (_heal)
        {
            health = _loadout.max_health;
        }
        else
        {
            if (health > _loadout.max_health)
                health = _loadout.max_health;
        }

        hud.SetHealthBarMaxHealth(_loadout.max_health);
        hud.UpdateHealthBar(health);
        movement_calculator.SetBaseSpeed(_loadout.move_speed);

        hat_renderer.sprite = _loadout.hat;
        transform.localScale = Vector3.one * _loadout.scale;
        shadow.orthographicSize = 1.5f * (_loadout.scale / 2);

        basic_ability.projectile_prefab = _loadout.basic_projectile;
        special_ability.projectile_prefab = _loadout.special_projectile;
    }


    public void BecomeTitan()
    {
        // Can't become titan during another Cataclysm event.
        if (GameManager.scene.pc_manager.PcState != PCState.None)
        {
            stats.target_energy = 99;
            return;
        }

        GameManager.scene.stat_tracker.LogTitanAchieved();
        AudioManager.PlayOneShot("titan_trigger");

        if (titan_aura == null)
        {
            var clone = Instantiate(LoadoutFactory.instance.titan_aura, this.transform);
            titan_aura = clone.GetComponent<TitanAura>();
            titan_aura.Init(this);
        }

        stats.target_energy = 100;
        shake_module.Shake(0.2f, 0.1f);
        rigid_body.mass = 30;
        Flash(Color.white);

        LoadoutFactory.AssignLoadout(this, "Gold");
        is_titan = true;

        GameManager.scene.pc_manager.TriggerTitanState(this);
    }


    public void Damage(int _damage, USBCharacter _dealer = null)
    {
        if (_damage <= 0)
            return;

        health -= _damage;
        hud.UpdateHealthBar(health);

        shake_module.Shake(0.2f, 0.1f);
        Flash(Color.white);

        if (health <= 0)
        {
            for (int i = 0; i < 3; ++i)
            {
                Projectile.CreateEffect(hit_particle,
                    body_group.transform.position, transform.position + (Vector3.up * 10));
            }

            GameManager.scene.stat_tracker.LogDeath(loadout_name);
            AudioManager.PlayOneShot("death");

            if (_dealer != null)
            {
                AwardKill(_dealer);
            }

            Destroy(this.gameObject);
        }
    }


    public void Damage(int _damage, Vector3 _hit_direction, USBCharacter _dealer = null)
    {
        Damage(_damage, _dealer);

        Projectile.CreateEffect(hit_particle,
            body_group.transform.position, _hit_direction);
    }


    public void Heal(int _amount)
    {
        health += _amount;
        Flash(Color.red);

        if (health >= loadout.max_health)
            health = loadout.max_health;

        hud.UpdateHealthBar(health);

        Projectile.CreateEffect(heal_particle,
            body_group.transform.position, transform.position + (Vector3.up * 10));
    }


    public void Stun(float _duration, bool _sound = true)
    {
        if (is_titan || _duration == 0)
            return;

        controls_disabled = true;
        stun_effect.SetActive(true);

        if (_sound)
            AudioManager.PlayOneShot("stun");

        CancelInvoke("RemoveStun");
        Invoke("RemoveStun", _duration);
    }


    public void Flash(Color _color, float _duration = 0.2f)
    {
        damage_flash.SetBaseColor(_color);
        damage_flash.FadeOut(_duration);
    }


    public void Init()
    {
        // Set ownership of its abilities.
        basic_ability.owner = this;
        special_ability.owner = this;

        // Initialise other components & systems.
        stats = new CharacterStats(this, hud, ref energy_lerp_speed, ref score_lerp_speed, ref energy_drain_rate);
        last_facing = transform.right;
        UpdateFaceIndicator();
        damage_flash.Init();
        original_mass = rigid_body.mass;
    }


    void Update()
    {
        // Determine when to flip the character.
        if ((last_facing.x > 0 && IsFlipped()) ||
            (last_facing.x < 0 && !IsFlipped()))
        {
            Flip();
        }

        // Play the walk cycle.
        animator.SetBool("walking", moving && !controls_disabled);
        moving = false; // Ensure walking anim never gets stuck.

        stats.Update();
    }


    void PowerDown()
    {
        LoadoutFactory.AssignLoadout(this, "Base", false);
        Flash(Color.yellow);

        AudioManager.PlayOneShot("new_data");
        Projectile.CreateEffect(LoadoutFactory.instance.download_data_prefab,
            transform.position, Vector3.zero);

        if (titan_aura != null)
            Destroy(titan_aura.gameObject);

        is_titan = false;
        rigid_body.mass = original_mass;
    }


    void FixedUpdate()
    {
        if (!slot_dropping)
        {
            Vector3 move = move_dir * movement_calculator.CalculateMoveSpeed() * Time.fixedDeltaTime;
            rigid_body.MovePosition(transform.position + move);

            move_dir = Vector3.zero;
        }

        if (rigid_body.velocity.magnitude >= velocity_limit)
        {
            rigid_body.velocity = rigid_body.velocity.normalized * velocity_limit;
        }
    }


    bool IsFlipped()
    {
        return body_group.transform.localScale.x < 0;
    }


    void Flip()
    {
        Vector3 scale = body_group.transform.localScale;
        scale.x = -scale.x;

        body_group.transform.localScale = scale;
    }


    void UpdateFaceIndicator()
    {
        Vector3 look_at = face_indicator.transform.position + (last_facing * 3);
        face_indicator.transform.LookAt(look_at);
        face_indicator.transform.Rotate(new Vector3(90, 0, 0));
    }


    void ScanForSlot()
    {
        last_slot_hit = null;
        var hits = Physics.BoxCastAll(transform.position, Vector3.one, Vector3.down);

        foreach (var hit in hits)
        {
            USBSlot slot = hit.collider.GetComponent<USBSlot>();

            if (slot == null || !slot.slottable ||
                (slot.golden_slot && !is_titan) ||
                (!slot.golden_slot) && is_titan)
            {
                continue;
            }

            slot.PostponeDeactivation();
            last_slot_hit = slot;
            transform.position = slot.transform.position;

            return;
        }
    }


    void FireSpecial()
    {
        if (controls_disabled)
            return;

        // Only fire slot drop ability while standing on a solid surface.
        if (!Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), -Vector3.up, 1,
            1 << LayerMask.NameToLayer("Prop") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Floor")))
        {
            return;
        }

        special_ability.Activate();
    }


    void SlotDropDone()
    {
        slot_dropping = false;

        if (last_slot_hit != null)
        {
            if (!last_slot_hit.slottable ||
                (Vector3.Distance(transform.position, last_slot_hit.transform.position) >= 1 * loadout.scale) ||
                controls_disabled)
            {
                last_slot_hit = null;
                return;
            }

            last_slot_hit.SlotDrop(this);

            if (!is_titan)
                GrantSlotDropBenefit();

            AudioManager.PlayOneShot("usb_slot");
        }

        last_slot_hit = null;
    }


    void RemoveStun()
    {
        controls_disabled = false;
        stun_effect.SetActive(false);
    }


    void GrantSlotDropBenefit()
    {
        if (stats.target_energy >= 100)
            return;

        stats.target_energy += energy_on_slot + (stats.target_score * energy_score_factor);
        stats.target_score = 0;
    }


    void AwardKill(USBCharacter _killer)
    {
        _killer.Heal(_killer.heal_on_kill);
        _killer.stats.target_score += score_on_kill;

        GameManager.scene.stat_tracker.LogKill(_killer.loadout_name);
        GameManager.scene.stat_tracker.LogScoreIncrease(_killer.loadout_name, score_on_kill);
    }
}