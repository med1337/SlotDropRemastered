using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePC : MonoBehaviour
{
    #region Public Vars
    public PcManager pc_manager;
    public Animator upgrade_slot_controller;
    public GameObject upgrade_hard_drive_prefab;
    public Vector3 spawn_location = new Vector3(0, 0, 10);
    public HandItemDropper hand_dropper;
    public float snap_speed = 1;
    public float door_close_delay = 1f;
    public float destroy_delay = 2f;
    public TransformFollower pc_upgrade_indicator;
    public GameObject upgrade_slot_indicator;
    #endregion

    #region Private Vars
    private Collider slot_trigger;
    private GameObject spawned_hard_drive = null;
    private bool move_to_slot = false;
    private const float LERP_TOLERANCE = 0.5f;
    private float t = 0;
    private bool upgrading = false;
    #endregion


    void Update()
    {
        SuckHardDrive();
    }


    public void TriggerUpgrade()
    {
        if (upgrading)
            return;
        
        upgrading = true;
        pc_manager.UpgradePc();
        OpenUpgradeSlot(true);
        SpawnUpgradeHardDrive();

        pc_upgrade_indicator.target = spawned_hard_drive.transform;
        upgrade_slot_indicator.SetActive(true);
    }


    public void ToggleUpgradeSlot()
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open",
            !upgrade_slot_controller.GetBool("upgrade_slot_open"));
    }


    private void OpenUpgradeSlot(bool _open_slot)
    {
        upgrade_slot_controller.SetBool("upgrade_slot_open", _open_slot);
    }


    private void SpawnUpgradeHardDrive()
    {
        if (upgrade_hard_drive_prefab)
        {
            spawned_hard_drive = Instantiate(upgrade_hard_drive_prefab);
            spawned_hard_drive.GetComponent<PropRespawner>().start_pos = spawn_location;
            spawned_hard_drive.GetComponent<PropRespawner>().start_rot = spawned_hard_drive.transform.rotation;

            PositionHardDrive();
        }
    }


    private void PositionHardDrive()
    {
        if (hand_dropper)
        {
            hand_dropper.PlaceObject(spawned_hard_drive);//place object with hand
        }
        else
        {
            spawned_hard_drive.transform.position = spawn_location;//fallback
        }
    }


    private void SuckHardDrive()
    {
        if (!move_to_slot)
            return;
             
        if (CheckHardDriveDistance())//check if move complete
        {
            EndHardDriveSuck();
            pc_upgrade_indicator.target = null;
            upgrade_slot_indicator.SetActive(false);

            return;
        }

        spawned_hard_drive.transform.position = Vector3.Lerp(spawned_hard_drive.transform.position,
            transform.position, t += Time.deltaTime * snap_speed);//lerp towards snap point
    }


    private bool CheckHardDriveDistance()
    {
        return (Vector3.Distance(spawned_hard_drive.transform.position, transform.position) <
                LERP_TOLERANCE);
    }


    private void EndHardDriveSuck()
    {
        move_to_slot = false;
        t = 0;

        Invoke("EndUpgrade", door_close_delay);
        DestroyHardDrive();
    }


    private void DestroyHardDrive()
    {
        if (spawned_hard_drive)
        {
            Destroy(spawned_hard_drive, destroy_delay);
            spawned_hard_drive = null;
        }
    }


    private void EndUpgrade()
    {
        upgrading = false;
        OpenUpgradeSlot(false);
        pc_manager.UpgradeOs();

        if (pc_manager)
            pc_manager.ResetProtectionBarToMax();
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
