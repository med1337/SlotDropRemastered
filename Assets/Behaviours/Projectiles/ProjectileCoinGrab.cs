using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCoinGrab : Projectile
{
    [Header("Parameters")]
    [SerializeField] float charge_force = 10;
    [SerializeField] float hit_force = 5;
    [SerializeField] float life_steal = 0.25f;
    [SerializeField] Vector3 particle_offset = new Vector3(0, 0.1f, 0);
    [SerializeField] Vector3 coin_offset = new Vector3(0, 5, 0);

    [Header("References")]
    [SerializeField] AudioClip coin_get;
    [SerializeField] List<AudioClip> hit_sounds;
    [SerializeField] GameObject coin_prefab;
    [SerializeField] GameObject particle_prefab;

    private List<USBCharacter> affected_characters = new List<USBCharacter>();
    private GameObject spawned_particle;

    
    void Start()
    {
        if (owner != null)
        {
            origin = owner.transform.position + particle_offset;

            owner.rigid_body.AddForce(facing * charge_force * 1000);
            spawned_particle = Projectile.CreateEffect(particle_prefab, origin, Vector3.zero); 
        }
    }


    void Update()
    {
        if (owner != null && owner.controls_disabled)
        {
            owner = null;
            Destroy(this.gameObject);

            return;
        }

        TrackPlayer();
    }


    void TrackPlayer()
    {
        if (owner != null)
        {
            origin = owner.transform.position + particle_offset;
            transform.position = origin;
            
            if (spawned_particle != null)
                spawned_particle.transform.position = origin;
        }
    }


    void OnTriggerEnter(Collider _other)
    {
        // Only collide with players.
        if (_other.tag != "USBCharacter")
            return;

        USBCharacter character = _other.GetComponent<USBCharacter>();

        // Don't collide with self.
        if (owner)
        {
            if (character == owner)
                return;
        }

        if (affected_characters.Contains(character))
            return;

        HitCharacter(character);
        SpawnCoin();
    }


    void HitCharacter(USBCharacter _character)
    {
        if (affected_characters.Contains(_character))
            return;

        AudioManager.PlayOneShot(hit_sounds[Random.Range(0, hit_sounds.Count - 1)]);

        Vector3 diff = (_character.body_group.transform.position - transform.position).normalized;
        _character.rigid_body.AddForce(diff * hit_force * 1000);
        
        if (owner != null)
        {
            owner.rigid_body.velocity = Vector3.zero;
            owner.rigid_body.AddForce(-diff * (hit_force / 2) * 1000);
            owner.Heal(Mathf.RoundToInt(damage * life_steal));
        }

        _character.Damage(damage, this.transform.position, owner);
        affected_characters.Add(_character);

        Destroy(this.gameObject);
    }


    void SpawnCoin()
    {
        if (owner == null)
            return;

        AudioManager.PlayOneShot(coin_get);

        var coin_clone = Instantiate(coin_prefab, owner.body_group.transform.position, Quaternion.identity);
        coin_clone.transform.Rotate(45, 0, 0);

        var coin = coin_clone.GetComponent<Coin>();
        coin.SetFollowTarget(owner.body_group.transform, coin_offset);
    }

}
