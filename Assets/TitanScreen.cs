using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitanScreen : MonoBehaviour
{
    public Text PointToWinText;
    public Slider PointSlider;
    public Text PointSliderText;

    [SerializeField] AudioSource hum_source;
    [SerializeField] AudioClip bar_maxed_clip;

    private bool _updateScore;
    private int _scoreTarget;

    // Use this for initialization
    void Start()
    {
        UpdateScreen();
    }

    // Update is called once per frame
    void Update()
    {
        hum_source.volume = AudioManager.sfx_volume * 1.5f;

        if (_updateScore)
        {
            if (PointSlider.value < _scoreTarget)
            {
                if (!hum_source.isPlaying)
                    hum_source.Play();

                hum_source.pitch = 1 + ((PointSlider.value / PointSlider.maxValue) * 2);

                int adjustment = Random.Range(1, 3);
                PointSlider.value += adjustment;

                PointSlider.value = Mathf.Clamp(PointSlider.value, 0, _scoreTarget);
            }
            else
            {
                if (hum_source.isPlaying)
                    hum_source.Stop();

                _updateScore = false;
            }

            PointToWinText.text = (PointSlider.maxValue - PointSlider.value) + " points\nto win";
            PointSliderText.text = PointSlider.value + "/" + PointSlider.maxValue;

            if (PointSlider.value >= PointSlider.maxValue)
            {
                _updateScore = false;

                GameManager.scene.slot_manager.enabled = false;
                GameManager.scene.general_canvas_manager.TriggerEndOfRound();

                PointToWinText.text = "Game Over";
                CameraShake.Shake(0.6f, 0.6f);

                if (hum_source.isPlaying)
                    hum_source.Stop();

                hum_source.pitch = 1;
                hum_source.PlayOneShot(bar_maxed_clip);
            }
        }
    }


    public void UpdateScreen()
    {
        _scoreTarget = GameManager.scene.pc_manager.TitanScore;
        _updateScore = true;
    }
}