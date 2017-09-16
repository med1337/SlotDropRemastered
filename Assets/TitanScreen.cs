using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitanScreen : MonoBehaviour
{
    public Text PointToWinText;
    public Slider PointSlider;
    public Text PointSliderText;
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
        if (_updateScore)
        {
            if (PointSlider.value < _scoreTarget)
            {
                PointSlider.value+=2;
            }
            else
            {
                _updateScore = false;
            }

            PointToWinText.text = (PointSlider.maxValue - PointSlider.value) + " points\nto win";
            PointSliderText.text = PointSlider.value + "/" + PointSlider.maxValue;

            if (PointSlider.value >= PointSlider.maxValue)
            {
                GameManager.round_over = true;
            }
        }
    }

    public void UpdateScreen()
    {
        _scoreTarget = GameManager.scene.pc_manager.TitanScore;
        _updateScore = true;
    }
}