using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Attach to a GameObject with an Image component or SpriteRenderer component.
public class FadableGraphic : MonoBehaviour
{
    public ListenerModule listener_module = new ListenerModule();

    private Image image;
    private SpriteRenderer sprite;

    private bool fading;
    private float fade_progress;
    private float fade_duration;

    private Color starting_color;
    private Color target_color;


    public void FadeColor(Color _from, Color _to, float _t)
    {
        starting_color = _from;
        target_color = _to;

        fade_duration = _t;

        StartFade();
    }


    public void FadeColor(Color _to, float _t)
    {
        FadeColor(GetGraphicColor(), _to, _t);
    }


    public void FadeAlpha(float _from, float _to, float _t)
    {
        Color from = GetGraphicColor();
        from.a = _from;

        Color to = from;
        to.a = _to;

        FadeColor(from, to, _t);
    }


    public void FadeAlpha(float _to, float _t)
    {
        FadeAlpha(GetGraphicColor().a, _to, _t);
    }


    public void FadeIn(float _t)
    {
        FadeAlpha(0, 1, _t);
    }


    public void FadeOut(float _t)
    {
        FadeAlpha(1, 0, _t);
    }


    void Start()
    {
        // Detect what sort of graphic we are.
        image = GetComponent<Image>();
        sprite = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (fading)
        {
            HandleFade();
        }
    }


    void HandleFade()
    {
        fade_progress += Time.deltaTime;
        Color color = Color.Lerp(starting_color, target_color, fade_progress / fade_duration);

        if (image)
        {
            image.color = color;
        }
        else if (sprite)
        {
            sprite.color = color;
        }

        // Determine if fade is complete.
        if (color == target_color)
        {
            StopFade();
            
            listener_module.NotifyListeners("FadableGraphicDone");
        }
    }


    void StartFade()
    {
        if (image)
        {
            image.color = starting_color;
        }
        else if (sprite)
        {
            sprite.color = starting_color;
        }

        fade_progress = 0;
        fading = true;
    }

    
    void StopFade()
    {
        fading = false;
        fade_progress = 0;
    }


    Color GetGraphicColor()
    {
        return image != null ? image.color : sprite.color;
    }

}
