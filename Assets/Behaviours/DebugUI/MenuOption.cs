using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MenuOptionType
{
    MUSIC_VOLUME,
    SFX_VOLUME,
    INCREASE_AI,
    DECREASE_AI,
    QUIT_GAME
}


public class MenuOption : MonoBehaviour
{
    public float value
    {
        get
        {
            if (slider == null)
                return 0;

            return slider.fillAmount;
        }

        set
        {
            if (slider != null)
                slider.fillAmount = value;
        }
    }

    [Header("Parameters")]
    public MenuOptionType option_type;
    [SerializeField] Color select_color;
    [SerializeField] Color deselect_color;
    [SerializeField] Color pressed_color;
    [SerializeField] Image slider;
    [SerializeField] string callback;
    [SerializeField] float press_fade_duration;

    [Header("References")]
    [SerializeField] Image bg;
    
    private FadableGraphic fade;


    public void Select()
    {
        bg.color = select_color;
    }


    public void Deselect()
    {
        bg.color = deselect_color;

        if (fade == null)
            Start();

        fade.FadeTo(deselect_color, 0);
    }


    public void Press()
    {
        if (callback == "")
            return;

        SendMessageUpwards(callback);

        if (fade == null)
            Start();

        fade.FadeColor(pressed_color, select_color, press_fade_duration);
    }


    public void AdjustValue(float _amount)
    {
        if (slider == null)
            return;

        slider.fillAmount = slider.fillAmount + _amount;
    }


    void Start()
    {
        fade = this.gameObject.AddComponent<FadableGraphic>();
    }

}
