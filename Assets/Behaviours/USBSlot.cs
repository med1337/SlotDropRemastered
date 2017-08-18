using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class USBSlot : MonoBehaviour
{
    public bool slottable { get { return fx_obj.activeSelf; } }

    [SerializeField] BoxCollider box_collider;
    [SerializeField] GameObject fx_obj;
    [SerializeField] float time_to_deactivate = 5.0f;


    public void Activate()
    {
        if (slottable)
            return;

        fx_obj.SetActive(true);

        Invoke("Deactivate", time_to_deactivate);
    }


    public void Deactivate()
    {
        CancelInvoke();

        fx_obj.SetActive(false);
    }


    public void PostponeDeactivation()
    {
        CancelInvoke();

        Invoke("Deactivate", time_to_deactivate / 2);
    }

    
    public void SlotDrop(USBCharacter _character)
    {
        if (!slottable)
            return;

        LoadoutFactory.AssignRandomLoadout(_character);
        _character.Flash();

        Deactivate();
    }


    void Start()
    {

    }


	void Update()
    {

	}

}
