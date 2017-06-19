using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBCharacter : MonoBehaviour
{
    public GameObject body_group;
    public Projector shadow;
    public Rigidbody rigid_body;
    public Animator animator;
    public GameObject face_indicator;
    public Renderer body_renderer;
    public GameObject stun_effect;

    public Vector3 last_facing { get; private set; }

    private USBLoadout loadout = new USBLoadout();
    private int health;

    private Ability basic_ability = new Ability();
    private Ability special_ability = new Ability();

    private Vector3 move_dir;
    private bool slot_dropping;
    private bool face_locked;


    public void Move(Vector3 _dir)
    {
        _dir.Normalize();

        if (_dir != Vector3.zero)
        {
            if (!face_locked)
                last_facing = _dir;
        }

        move_dir = _dir;

        UpdateFaceIndicator();
    }


    public void Attack()
    {
        // TODO: perform basic ability ..
    }


    public void SlotDrop()
    {
        if (slot_dropping)
            return;

        // TODO: check slotdrop ability is off cooldown ..

        slot_dropping = true;
        animator.SetTrigger("slot_drop");
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
        body_group.transform.localScale = _loadout.scale;
        shadow.orthographicSize = _loadout.scale.x;
    }


    public void Damage(int _damage)
    {

    }


    public void Stun(float _duration)
    {

    }


    void Start()
    {
        // Set ownership of its abilities.
        basic_ability.owner = this;
        special_ability.owner = this;

        // Set initial rotation of the face indicator.
        last_facing = transform.right;
        UpdateFaceIndicator();
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
        animator.SetBool("walking", move_dir != Vector3.zero);
    }


    void FixedUpdate()
    {
        if (!slot_dropping)
        {
            Vector3 move = move_dir * loadout.move_speed * Time.fixedDeltaTime;
            rigid_body.MovePosition(transform.position + move);
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
        // TODO: perform special ability ..
    }


    void SlotDropDone()
    {
        slot_dropping = false;
    }

}
