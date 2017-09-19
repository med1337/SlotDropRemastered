﻿using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GeneralCanvasManager : MonoBehaviour
{
    public GameObject title_panel;
    public GameObject players_needed_prompt;
    public GameObject results_panel;

    [SerializeField] float time_before_title = 3;
    [SerializeField] float time_before_results = 3;


    public void TriggerEndOfRound()
    {
        StartCoroutine(EndOfRound());
    }


    IEnumerator EndOfRound()
    {
        Time.timeScale = 0.3f;
        GameManager.restarting_scene = true;

        yield return new WaitForSecondsRealtime(time_before_title);

        Time.timeScale = 1;
        title_panel.SetActive(true);

        yield return new WaitForSecondsRealtime(time_before_results);

        PlayerManager.IdleAllPlayers();
        GameManager.scene.respawn_manager.KillAllCharacters();
        AudioManager.StopAllSFX();

        results_panel.GetComponent<ResultsPanelManager>().UpdateSessionTimer();
        results_panel.SetActive(true);

        yield return new WaitUntil(() => !results_panel.activeSelf);

        GameManager.restarting_scene = false;
        title_panel.SetActive(false);

        SceneManager.LoadScene(0);
    }


	
    void Update()
    {
        //results_panel.SetActive(Input.GetKey(KeyCode.Tab));

        bool players_needed = GameManager.scene.respawn_manager.MorePlayersNeeded() && !GameManager.restarting_scene;
        players_needed_prompt.SetActive(players_needed);
    }

}
