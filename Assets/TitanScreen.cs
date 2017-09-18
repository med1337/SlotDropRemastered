using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
                _updateScore = false;
                GameManager.scene.slot_manager.enabled = false;

                StartCoroutine(EndOfRound());
            }
        }
    }


    IEnumerator EndOfRound()
    {
        Time.timeScale = 0.3f;
        GameManager.restarting_scene = true;

        yield return new WaitForSecondsRealtime(3);

        Time.timeScale = 1;
        GameManager.instance.end_game_canvas.SetActive(true);

        yield return new WaitForSecondsRealtime(3);

        GameManager.restarting_scene = false;
        GameManager.instance.end_game_canvas.SetActive(false);

        PlayerManager.IdleAllPlayers();
        AudioManager.StopAllSFX();

        SceneManager.LoadScene(0);
    }


    public void UpdateScreen()
    {
        _scoreTarget = GameManager.scene.pc_manager.TitanScore;
        _updateScore = true;
    }
}