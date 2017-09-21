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
            if (_other.tag == "PCUpgrade")
            {
                GameManager.scene.pc_manager.CancelUpgradeState();
                GameManager.scene.stat_tracker.LogFailedUpgrade();

                var colliders = _other.GetComponents<Collider>();
                foreach (Collider c in colliders)
                    Destroy(c);
            }
            else
            {
                PropRespawner prop = _other.GetComponent<PropRespawner>();

                if (prop == null)
                    prop = _other.GetComponentInParent<PropRespawner>();

                if (prop == null)
                    prop = _other.transform.parent.GetComponentInParent<PropRespawner>();

                prop.RespawnProp();
            }
        }
    }

}
