using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePC : MonoBehaviour
{
    public PcManager pc_manager;
    public Animator upgrade_slot_controller;
    public GameObject upgrade_hard_drive_prefab;
    public Vector3 spawn_location = new Vector3(0, 0, 10);
    public float snap_speed = 1;
    public float door_close_delay = 1f;
    public float destroy_delay = 2f;

    private Collider slot_trigger;
    private GameObject spawned_hard_drive = null;
    private bool move_to_slot = false;
    private const float LERP_TOLERANCE = 0.5f;
    private float t = 0;
    private bool upgrading = false;


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
            TriggerUpgrade();

        SuckHardDrive();
    }


    void EndUpgrade()
    {
        upgrading = false;

        OpenUpgradeSlot(false);

        if (pc_manager)
            pc_manager.ResetProtectionBarToMax();
    }


    public void TriggerUpgrade()
    {
        if (upgrading)
            return;
        
        upgrading = true;
        OpenUpgradeSlot(true);
        SpawnUpgradeHardDrive();
    }


    public void ToggleUpgradeSlot()
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open",
            !upgrade_slot_controller.GetBool("upgrade_slot_open"));
    }


    void OpenUpgradeSlot(bool _open_slot)
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open", _open_slot);
    }


    void SpawnUpgradeHardDrive()
    {
        if (upgrade_hard_drive_prefab)
        {
            spawned_hard_drive = Instantiate(upgrade_hard_drive_prefab);
            spawned_hard_drive.transform.position = spawn_location;
        }
    }


    void SuckHardDrive()
    {
        if (move_to_slot)
        {
            //check if move complete
            if (Vector3.Distance(spawned_hard_drive.transform.position, transform.position) <
                LERP_TOLERANCE)
            {
                move_to_slot = false;
                t = 0;

                Invoke("EndUpgrade", door_close_delay);

                if (spawned_hard_drive)
                {
                    Destroy(spawned_hard_drive, destroy_delay);
                    spawned_hard_drive = null;
                }

                return;
            }

            spawned_hard_drive.transform.position = Vector3.Lerp(spawned_hard_drive.transform.position,
                transform.position, t += Time.deltaTime * snap_speed);//lerp towards snap point
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (spawned_hard_drive == null)
            return;
        
        if (other.gameObject.name == spawned_hard_drive.gameObject.name)
        {
            move_to_slot = true;
        }
    }

}
