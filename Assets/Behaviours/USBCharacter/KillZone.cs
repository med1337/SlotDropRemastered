using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    void OnTriggerEnter(Collider _other)
    {
        if (_other.tag == "USBCharacter")
        {
            USBCharacter character = _other.GetComponent<USBCharacter>();
            character.Damage(int.MaxValue);
        }
        else if (_other.gameObject.layer == LayerMask.NameToLayer("Prop"))
        {
            PropRespawner prop = _other.GetComponent<PropRespawner>();

            if (prop == null)
                _other.GetComponentInParent<PropRespawner>();

            if (prop == null)
                prop = _other.transform.parent.GetComponentInParent<PropRespawner>();

            prop.RespawnProp();
        }
    }

}
