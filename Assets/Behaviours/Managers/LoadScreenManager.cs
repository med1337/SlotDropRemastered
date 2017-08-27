using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadScreenManager : MonoBehaviour
{
    private enum LoadScreenState
    {
        PREPARING,
        LOADING,
        DELAYING,
        DONE
    }

    [SerializeField] Image progress_bar;

    private const float LOAD_DELAY = 3.0f;

    private LoadScreenState state;
    private AsyncOperation load_task;
    private float load_timer;
    private bool bar_refresh_needed;


    void Update()
    {
        switch (state)
        {
            case LoadScreenState.PREPARING:
            {
                load_task = SceneManager.LoadSceneAsync(1);
                load_task.allowSceneActivation = false;
                
                state = LoadScreenState.LOADING;
            } break;

            case LoadScreenState.LOADING:
            {
                if (load_task == null)
                    return;

                if (LoadTaskDone())
                {
                    state = LoadScreenState.DELAYING;

                    bar_refresh_needed = true;
                }
            } break;

            case LoadScreenState.DELAYING:
            {
                load_timer += Time.deltaTime;

                ProgressBarStutter();

                if (load_timer >= LOAD_DELAY)
                {
                    state = LoadScreenState.DONE;

                    StartCoroutine(DelayDone());
                }
            } break;
        }
    }


    bool LoadTaskDone()
    {
        return (load_task.progress >= 0.8f || load_task.isDone);
    }


    void ProgressBarStutter()
    {
        if (!bar_refresh_needed)
            return;

        bar_refresh_needed = false;
        Invoke("UpdateProgressBar", Random.Range(LOAD_DELAY / 12, LOAD_DELAY / 6));
    }


    void UpdateProgressBar()
    {
        progress_bar.fillAmount = load_timer / LOAD_DELAY;
        bar_refresh_needed = true;
    }


    IEnumerator DelayDone()
    {
        CancelInvoke();

        this.enabled = false;
        progress_bar.fillAmount = 1;

        yield return new WaitForSeconds(LOAD_DELAY / 2);

        load_task.allowSceneActivation = true;
    }
}
