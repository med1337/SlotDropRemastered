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
    private USBSlot last_opened_slot;


    public void ActivateTitanSlots()
    {
        foreach (USBSlot slot in slots)
        {
            if (!slot.golden_slot)
                continue;

            if (GameManager.scene.respawn_manager.titan_exists)
            {
                slot.Activate();
            }
            else
            {
                slot.Deactivate();
            }
        }
    }


    void Start()
    {
		slots = GameObject.FindObjectsOfType<USBSlot>().ToList();
    }


    void OnEnable()
    {
        if (slots == null)
            return;

        foreach (USBSlot slot in slots)
            slot.ShowDisabledIndicator(false);
    }

    
    void OnDisable()
    {
        if (slots == null)
            return;

        CancelInvoke();
        DeactivateAllSlots();

        foreach (USBSlot slot in slots)
            slot.ShowDisabledIndicator(true);
    }
	

	void Update()
    {
		if (!random_slot_queued)
        {
            float delay = Random.Range(min_activate_delay, max_activate_delay);

            Invoke("OpenRandomSlot", delay);
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


    void DeactivateAllSlots()
    {
        slots.RemoveAll(slot => slot == null);

        foreach (USBSlot slot in slots)
        {
            slot.Deactivate();
        }

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

        } while (slot.slottable || slot.golden_slot || slot == last_opened_slot);

        if (slot != null)
        {
            slot.Activate();
            last_opened_slot = slot;
        }

        random_slot_queued = false;
    }

}
