using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    [SerializeField] float min_activate_delay = 2;
    [SerializeField] float max_activate_delay = 10;

    private USBSlot[] slots;
    private bool random_slot_queued = false;
    private PlayerManager player_manager;

    bool invoke_called = false;


	void Start()
    {
        player_manager = GameObject.FindObjectOfType<PlayerManager>();
		slots = GameObject.FindObjectsOfType<USBSlot>();
	}
	

	void Update()
    {
		if (!random_slot_queued)
        {
            Invoke("OpenRandomSlot", Random.Range(min_activate_delay, max_activate_delay));
            random_slot_queued = true;
        }
	}


    void DeactivateAllSlots()
    {
        foreach (USBSlot slot in slots)
        {
            slot.Deactivate();
        }
    }


    void OpenRandomSlot()
    {
        USBSlot slot = null;
        int activate_attempts = 0;

        do
        {
            slot = slots[Random.Range(0, slots.Length)];

            if (activate_attempts++ >= 10)
                break;

        } while (slot.slottable);

        if (slot != null)
            slot.Activate();

        random_slot_queued = false;
    }

}
