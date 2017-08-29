﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;

public class USBCharacter : MonoBehaviour
{
    public Vector3 last_facing { get; private set; }
    public string loadout_name { get { return loadout.name; } }

    public GameObject body_group;
    public Rigidbody rigid_body;
    public float move_speed_modifier = 1;
    public Vector3 move_dir;

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
    private int slot_streak;


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
        Damage(0);

        slot_tracker.gameObject.SetActive(false);
        LoadoutFactory.AssignLoadout(this, "Gold");
    }


    public void Damage(int _damage)
    {
        health -= _damage;

        hud.UpdateHealthBar(health);
        shake_module.Shake(0.2f, 0.1f);
        Flash();

        if (health <= 0)
        {
            for (int i = 0; i < 3; ++i)
            {
                Projectile.CreateEffect(hit_particle,
                    body_group.transform.position, transform.position + Vector3.up * 5);
            }

            AudioManager.PlayOneShot("death");
            Destroy(this.gameObject);
        }
    }


    public void Damage(int _damage, Vector3 _hit_direction)
    {
        Damage(_damage);

        Projectile.CreateEffect(hit_particle,
            body_group.transform.position, _hit_direction);
    }


    public void Stun(float _duration, bool _sound = true)
    {
        controls_disabled = true;
        stun_effect.SetActive(true);

        if (_sound)
            AudioManager.PlayOneShot("stun");

        Invoke("RemoveStun", _duration);
    }


    public void Flash(float _duration = 0.2f)
    {
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
            
            if (slot == null || !slot.slottable || (slot.golden_slot && loadout_name != "Gold"))
                continue;

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
                Vector3.Distance(transform.position, last_slot_hit.transform.position) >= 1 * loadout.scale ||
                controls_disabled)
            {
                last_slot_hit = null;
                return;
            }

            last_slot_hit.SlotDrop(this);
            IncrementSlotTracker();

            AudioManager.PlayOneShot("usb_slot");
        }
        
        last_slot_hit = null;
    }


    void RemoveStun()
    {
        controls_disabled = false;
        stun_effect.SetActive(false);
    }


    void IncrementSlotTracker()
    {
        if (slot_streak == 5)
            return;

        ++slot_streak;

        for (int i = 0; i < slot_tracker.childCount; ++i)
            slot_tracker.GetChild(i).gameObject.SetActive(slot_streak > i);

        if (slot_streak == 5)
        {
            BecomeTitan();
        }
    }

}
