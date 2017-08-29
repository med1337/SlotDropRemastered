using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider health_bar;
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


	public void UpdateSlotProgress(int number)
    {
        for (int i = 0; i < number; ++i)
        {
            Color color = slot_drop_tokens[i].color;
            color.a = 1;

            slot_drop_tokens[i].color = color;
        }
    }


    void Start()
    {
        UpdateHealthBar((int)health_bar.maxValue);
    }

}
