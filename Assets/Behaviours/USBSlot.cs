using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBSlot : MonoBehaviour
{
    public bool slottable { get { return fx_obj.activeSelf; } }
    public bool golden_slot;

    [SerializeField] BoxCollider box_collider;
    [SerializeField] GameObject fx_obj;
    [SerializeField] float time_to_deactivate = 5.0f;


    public void Activate()
    {
        if (slottable)
            return;

        fx_obj.SetActive(true);

        if (!golden_slot)
            Invoke("Deactivate", time_to_deactivate);
    }


    public void Deactivate()
    {
        CancelInvoke();

        fx_obj.SetActive(false);
    }


    public void PostponeDeactivation()
    {
        if (golden_slot)
            return;

        CancelInvoke();

        Invoke("Deactivate", time_to_deactivate / 2);
    }

    
    public void SlotDrop(USBCharacter _character)
    {
        if (!slottable)
            return;

        if (_character.loadout_name == "Gold" && golden_slot)
        {
            GameManager.round_over = true;
            GameManager.scene.focus_camera.Focus(transform.position, 15, 10, false);
        }
        else
        {
            LoadoutFactory.AssignRandomLoadout(_character);
            _character.Flash(Color.yellow);

            AudioManager.PlayOneShot("new_data");
            Projectile.CreateEffect(LoadoutFactory.instance.download_data_prefab,
                _character.transform.position, Vector3.zero);

            GameManager.scene.pc_manager.AttemptQuarantine();
        }

        Deactivate();
    }


    void Start()
    {

    }


	void Update()
    {

	}

}
