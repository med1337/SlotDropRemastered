using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public Slider health_bar;
    public List<Image> slot_drop_tokens = new List<Image>();
    public int active_slot_token_count = 0;


    private void Start()
    {
        ResetSlotTokens();
        UpdateHealthBar((int)health_bar.maxValue);
    }


    public void SetHealthBarMaxHealth(int max_health)
    {
        health_bar.maxValue = max_health;
        health_bar.value = max_health;
    }


    public void UpdateHealthBar(int health)
    {
        health_bar.value = health;
    }


	public int AddSlotToken()
    {
        if (active_slot_token_count < slot_drop_tokens.Count)
        {
            slot_drop_tokens[active_slot_token_count].gameObject.SetActive(true);
            ++active_slot_token_count;
        }

        return active_slot_token_count; //returns the count of active streak tokens for checking if streak is complete
    }


    public void ResetSlotTokens()
    {
        active_slot_token_count = 0;

        foreach (Image token in slot_drop_tokens)
        {
            token.gameObject.SetActive(false);
        }
    }
}
