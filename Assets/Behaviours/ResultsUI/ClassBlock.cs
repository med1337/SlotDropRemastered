using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassBlock : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Sprite portrait;
    [SerializeField] string class_name = "";

    [Header("References")]
    [SerializeField] Image class_portrait;
    [SerializeField] Text class_text;
    [SerializeField] Slider kills_bar;
    [SerializeField] Slider deaths_bar;
    [SerializeField] Slider score_earned_bar;
    [SerializeField] Slider score_deposited_bar;
    [SerializeField] Text kills_text;
    [SerializeField] Text deaths_text;
    [SerializeField] Text score_earned_text;
    [SerializeField] Text score_deposited_text;

    private LoadoutStats attached_stats;


    void Awake()
    {
        class_portrait.sprite = portrait;
        class_text.text = class_name;

        kills_bar.value = 0;
        deaths_bar.value = 0;
        score_earned_bar.value = 0;
        score_deposited_bar.value = 0;
    }


    void Update()
    {
        if (attached_stats == null && class_name != "")
            attached_stats = GameManager.scene.stat_tracker.loadout_stats[class_name];

        if (attached_stats == null)
            return;

        if (kills_bar.value != attached_stats.kills)
        {
            float adjustment = Mathf.Abs(kills_bar.value - kills_bar.maxValue) * Time.unscaledDeltaTime;

            if (adjustment < 1)
                adjustment = 1;

            kills_bar.value = Mathf.Clamp(kills_bar.value + adjustment, 0, attached_stats.kills);
        }

        if (deaths_bar.value != attached_stats.deaths)
        {
            float adjustment = Mathf.Abs(deaths_bar.value - deaths_bar.maxValue) * Time.unscaledDeltaTime;

            if (adjustment < 1)
                adjustment = 1;

            deaths_bar.value = Mathf.Clamp(deaths_bar.value + adjustment, 0, attached_stats.deaths);
        }

        if (score_earned_bar.value != attached_stats.score_earned)
        {
            float adjustment = Mathf.Abs(score_earned_bar.value - score_earned_bar.maxValue) * Time.unscaledDeltaTime;

            if (adjustment < 1)
                adjustment = 1;

            score_earned_bar.value = Mathf.Clamp(score_earned_bar.value + adjustment, 0, attached_stats.score_earned);
        }

        if (score_deposited_bar.value != attached_stats.score_deposited)
        {
            float adjustment = Mathf.Abs(score_deposited_bar.value - score_deposited_bar.maxValue) * Time.unscaledDeltaTime;

            if (adjustment < 1)
                adjustment = 1;

            score_deposited_bar.value = Mathf.Clamp(score_deposited_bar.value + adjustment, 0, attached_stats.score_deposited);
        }

        UpdateBarMaxValues();
        UpdateBarText();
    }


    void UpdateBarMaxValues()
    {
        kills_bar.maxValue = GameManager.scene.stat_tracker.largest_stats.kills;
        deaths_bar.maxValue = GameManager.scene.stat_tracker.largest_stats.deaths;
        score_earned_bar.maxValue = GameManager.scene.stat_tracker.largest_stats.score_earned;
        score_deposited_bar.maxValue = GameManager.scene.stat_tracker.largest_stats.score_deposited;
    }


    void UpdateBarText()
    {
        kills_text.text = kills_bar.value.ToString("F0");
        deaths_text.text = deaths_bar.value.ToString("F0");
        score_earned_text.text = score_earned_bar.value.ToString("F0");
        score_deposited_text.text = score_deposited_bar.value.ToString("F0");
    }

}
