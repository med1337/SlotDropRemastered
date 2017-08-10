using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{

    void OnTriggerEnter(Collider _other)
    {
        if (_other.tag != "USBCharacter")
            return;

        USBCharacter character = _other.GetComponent<USBCharacter>();
        character.Damage(int.MaxValue);
    }

}
