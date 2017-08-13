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

    private USBLoadout loadout = new USBLoadout();
    private int health;

    private Ability basic_ability = new Ability();
    private Ability special_ability = new Ability();

    private Vector3 move_dir;
    private bool moving;
    private bool slot_dropping;
    private bool face_locked;
    private bool controls_disabled;


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
        if (slot_dropping || controls_disabled)
            return;

        basic_ability.Activate();
    }


    public void SlotDrop()
    {
        if (slot_dropping || controls_disabled)
            return;

        if (special_ability.IsReady())
        {
            slot_dropping = true;
            animator.SetTrigger("slot_drop");
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
        transform.localScale = _loadout.scale;
        shadow.orthographicSize = 1.5f * (_loadout.scale.x / 2);

        basic_ability.projectile_prefab = _loadout.basic_projectile;
        special_ability.projectile_prefab = _loadout.special_projectile;
    }


    public void Damage(int _damage)
    {
        health -= _damage;

        hud.UpdateHealthBar(health);
        damage_flash.FadeOut(0.2f);
        shake_module.Shake(0.2f, 0.1f);

        if (health <= 0)
            Destroy(this.gameObject);
    }


    public void Damage(int _damage, Vector3 _hit_direction)
    {
        Damage(_damage);

        Projectile.CreateEffect(hit_particle,
            body_group.transform.position, _hit_direction);
    }


    public void Stun(float _duration)
    {
        controls_disabled = true;
        stun_effect.SetActive(true);
        shake_module.Shake(0.2f, 0.1f);

        Invoke("RemoveStun", _duration);
    }


    void Start()
    {
        // Set ownership of its abilities.
        basic_ability.owner = this;
        special_ability.owner = this;

        // Set initial rotation of the face indicator.
        last_facing = transform.right;
        UpdateFaceIndicator();
        
        // Set initial debug loadout.
        AssignLoadout(loadout);
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


    void FireSpecial()
    {
        special_ability.Activate();
    }


    void SlotDropDone()
    {
        slot_dropping = false;
    }


    void RemoveStun()
    {
        controls_disabled = false;
        stun_effect.SetActive(false);
    }

}
