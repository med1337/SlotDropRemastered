using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USBSlot : MonoBehaviour
{
    public bool slottable
    {
        get { return fx_obj.activeSelf; }
    }

    public bool golden_slot;

    [SerializeField] BoxCollider box_collider;
    [SerializeField] GameObject fx_obj;
    [SerializeField] float time_to_deactivate = 5.0f;
    [SerializeField] FadableGraphic disabled_indicator;


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


    public void ShowDisabledIndicator(bool _show)
    {
        if (disabled_indicator != null)
            disabled_indicator.gameObject.SetActive(_show);
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

        if (_character.is_titan && golden_slot)
        {
            GameManager.scene.stat_tracker.LogScoreDeposited(_character.loadout_name, _character.stats.target_score);
            _character.Flash(Color.yellow);
            AudioManager.PlayOneShot("new_data");
            Projectile.CreateEffect(LoadoutFactory.instance.download_data_prefab,
                _character.transform.position, Vector3.zero);
            
            GameManager.scene.pc_manager.DepositScore(_character.stats.score);
            _character.stats.target_score = 0;
        }
        else
        {
            GameManager.scene.stat_tracker.LogScoreDeposited(_character.loadout_name, _character.stats.target_score);

            LoadoutFactory.AssignRandomLoadout(_character);
            _character.Flash(Color.yellow);

            AudioManager.PlayOneShot("new_data");
            Projectile.CreateEffect(LoadoutFactory.instance.download_data_prefab,
                _character.transform.position, Vector3.zero);

            GameManager.scene.pc_manager.AttemptQuarantine(_character);
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