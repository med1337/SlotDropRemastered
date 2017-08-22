using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SlotManager : MonoBehaviour
{
    [SerializeField] float min_activate_delay = 2;
    [SerializeField] float max_activate_delay = 10;

    private List<USBSlot> slots;
    private bool random_slot_queued = false;

    bool invoke_called = false;


	void Start()
    {
		slots = GameObject.FindObjectsOfType<USBSlot>().ToList();
    }

    
    void OnDisable()
    {
        CancelInvoke();
        DeactivateAllSlots();
    }
	

	void Update()
    {
		if (!random_slot_queued)
        {
            Invoke("OpenRandomSlot", Random.Range(min_activate_delay, max_activate_delay));
            random_slot_queued = true;
        }

        if (GameManager.restarting_scene)
        {
            DeactivateAllSlots();
        }
        else
        {
            ActivateTitanSlots();
        }
	}


    void ActivateTitanSlots()
    {
        bool titan_exists = false;

        foreach (USBCharacter character in RespawnManager.current_players)
        {
            if (character.loadout_name == "Gold")
            {
                titan_exists = true;
                break;
            }
        }

        foreach (USBSlot slot in slots)
        {
            if (!slot.golden_slot)
                continue;

            if (titan_exists)
            {
                slot.Activate();
            }
            else
            {
                slot.Deactivate();
            }
        }
    }



    void DeactivateAllSlots()
    {
        slots.RemoveAll(slot => slot == null);

        foreach (USBSlot slot in slots)
            slot.Deactivate();

        random_slot_queued = false;
    }


    void OpenRandomSlot()
    {
        if (GameManager.restarting_scene)
            return;

        USBSlot slot = null;
        int activate_attempts = 0;

        do
        {
            slot = slots[Random.Range(0, slots.Count)];

            if (activate_attempts++ >= 10)
                break;

        } while (slot.slottable || slot.golden_slot);

        if (slot != null)
            slot.Activate();

        random_slot_queued = false;
    }

}
