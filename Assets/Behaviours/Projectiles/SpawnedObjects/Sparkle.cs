using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sparkle : MonoBehaviour
{
    public GameObject fire_block_prefab;
    public Rigidbody rigid_body;

    private USBCharacter owner;


    public void Init(USBCharacter _owner, Vector3 _force)
    {
        owner = _owner;
        rigid_body = GetComponent<Rigidbody>();

        rigid_body.AddForce(_force);
    }


    void OnTriggerEnter(Collider _other)
    {
        if (owner != null && _other.transform.GetInstanceID() == owner.transform.GetInstanceID() ||
            _other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
            return;

        var clone = Instantiate(fire_block_prefab, transform.position, Quaternion.identity);
        clone.AddComponent<TempParticle>();

        clone.GetComponent<FireBlock>().Init(owner, 5, 0.5f, Vector3.one * 2.5f, Vector3.zero);
        CameraShake.Shake(0.3f, 0.3f);

        Destroy(this.gameObject);
    }

}
