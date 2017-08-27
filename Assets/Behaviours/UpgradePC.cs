using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePC : MonoBehaviour
{
    public Animator upgrade_slot_controller;
    public GameObject upgrade_block_prefab;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
           ToggleUpgradeSlot();
    }


    void UpgradedPC(UpgradeBlock update_block)
    {
        OpenUpgradeSlot(false);
    }


    public void TriggerUpgrade()
    {
        OpenUpgradeSlot(true);
        SpawnUpgradeCube();
    }

    void ToggleUpgradeSlot()
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open",
            !upgrade_slot_controller.GetBool("upgrade_slot_open"));
    }


    void OpenUpgradeSlot(bool _open_slot)
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open", _open_slot);
    }


    void SpawnUpgradeCube()
    {
        UpgradeBlock clone = Instantiate(upgrade_block_prefab).GetComponent<UpgradeBlock>();
        clone.listener_module.AddListener(this.gameObject);
    }
}
