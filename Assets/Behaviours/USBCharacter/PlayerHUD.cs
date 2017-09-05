using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider health_bar;
    public Text score_text;
    public ShakeModule text_shake;
    public FadableGraphic text_fade;
    public List<Image> slot_drop_tokens = new List<Image>();
    public int active_slot_token_count = 0;


    public void SetHealthBarMaxHealth(int max_health)
    {
        health_bar.maxValue = max_health;
        health_bar.value = max_health;
    }


    public void UpdateHealthBar(int health)
    {
        health_bar.value = health;
    }


    public void UpdateEnergy(float _energy)
    {
        if (_energy == 0)
        {

        }
        else if (_energy < 25)
        {
            slot_drop_tokens[0].fillAmount = _energy / 25;
            slot_drop_tokens[1].fillAmount = 0;
            slot_drop_tokens[2].fillAmount = 0;
            slot_drop_tokens[3].fillAmount = 0;
        }
        else if (_energy < 50)
        {
            slot_drop_tokens[0].fillAmount = 1;
            slot_drop_tokens[1].fillAmount = (_energy - 25) / 25;
            slot_drop_tokens[2].fillAmount = 0;
            slot_drop_tokens[3].fillAmount = 0;
        }
        else if (_energy < 75)
        {
            slot_drop_tokens[0].fillAmount = 1;
            slot_drop_tokens[1].fillAmount = 1;
            slot_drop_tokens[2].fillAmount = (_energy - 50) / 25;
            slot_drop_tokens[3].fillAmount = 0;
        }
        else if (_energy < 100)
        {
            slot_drop_tokens[0].fillAmount = 1;
            slot_drop_tokens[1].fillAmount = 1;
            slot_drop_tokens[2].fillAmount = 1;
            slot_drop_tokens[3].fillAmount = (_energy - 75) / 25;
        }
    }


    public void UpdateScore(int number)
    {
        score_text.text = number.ToString();
        text_shake.Shake(0.3f, 0.3f);
        
        text_fade.FadeColor(Color.green, Color.white, 0.5f);
    }


    void Start()
    {
        UpdateHealthBar((int)health_bar.maxValue);
    }

}
