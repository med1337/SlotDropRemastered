using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCharacter : MonoBehaviour
{
    public Vector3 last_facing { get; private set; }
    public string loadout_name { get { return loadout.name; } }

    public GameObject body_group;
    public Rigidbody rigid_body;
    public float move_speed_modifier = 1;
    public Vector3 move_dir;
    public int heal_on_kill = 20;
    public int score
    {
        get { return score_; }
        set { score_ = value; hud.UpdateScoreText(score_); }
    }

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

    private USBLoadout loadout = new USBLoadout();
    private int health;

    private Ability basic_ability = new Ability();
    private Ability special_ability = new Ability();

    private bool moving;
    private bool slot_dropping;
    private bool face_locked;
    private bool controls_disabled;
    private USBSlot last_slot_hit;
    private float energy;
    private int score_;


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


    public void AssignLoadout(USBLoadout _loadout)
    {
        // TODO: remove previous particle effect ..

        loadout = _loadout;
        health = _loadout.max_health;
        hud.SetHealthBarMaxHealth(_loadout.max_health);
        hud.UpdateHealthBar(health);

        hat_renderer.sprite = _loadout.hat;
        transform.localScale = Vector3.one * _loadout.scale;
        shadow.orthographicSize = 1.5f * (_loadout.scale / 2);

        basic_ability.projectile_prefab = _loadout.basic_projectile;
        special_ability.projectile_prefab = _loadout.special_projectile;
    }


    public void BecomeTitan()
    {
        AudioManager.PlayOneShot("titan_trigger");
        var clone = Instantiate(LoadoutFactory.instance.titan_aura, this.transform);
        clone.GetComponent<TitanAura>().Init(this);

        Damage(0);

        energy = 100;
        LoadoutFactory.AssignLoadout(this, "Gold");
    }


    public void Damage(int _damage, USBCharacter _dealer = null)
    {
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

            AudioManager.PlayOneShot("death");

            if (_dealer != null)
            {
                _dealer.Heal(_dealer.heal_on_kill);
                _dealer.score += 50;
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
        if (loadout_name == "Gold")
            return;

        controls_disabled = true;
        stun_effect.SetActive(true);

        if (_sound)
            AudioManager.PlayOneShot("stun");

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
        last_facing = transform.right;
        UpdateFaceIndicator();
        damage_flash.Init();
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

        DrainEnergy();
    }


    void DrainEnergy()
    {
        if (energy == 0)
            return;

        energy -= 1.25f * Time.deltaTime;
        energy = Mathf.Clamp(energy, 0, 100);

        hud.UpdateEnergy(energy);

        if (energy == 0)
        {
            LoadoutFactory.AssignLoadout(this, "Base");
        }
    }


    void FixedUpdate()
    {
        if (!slot_dropping)
        {
            Vector3 move = move_dir * loadout.move_speed * Time.fixedDeltaTime;
            rigid_body.MovePosition(transform.position + move * move_speed_modifier);

            move_dir = Vector3.zero;
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
                (slot.golden_slot && loadout_name != "Gold") ||
                (!slot.golden_slot) && loadout_name == "Gold")
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
        special_ability.Activate();
    }


    void SlotDropDone()
    {
        slot_dropping = false;
        
        if (last_slot_hit != null)
        {
            if (!last_slot_hit.slottable ||
                (Vector3.Distance(transform.position, last_slot_hit.transform.position) >= 1 * loadout.scale) ||
                stun_effect.activeSelf)
            {
                last_slot_hit = null;
                return;
            }

            last_slot_hit.SlotDrop(this);
            IncreaseEnergy();

            AudioManager.PlayOneShot("usb_slot");
        }
        
        last_slot_hit = null;
    }


    void RemoveStun()
    {
        controls_disabled = false;
        stun_effect.SetActive(false);
    }


    void IncreaseEnergy()
    {
        if (energy >= 100)
            return;

        energy += 25;

        if (energy >= 100)
        {
            BecomeTitan();
        }
    }

}
