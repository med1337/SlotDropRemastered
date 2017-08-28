using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool round_over;
    public static bool restarting_scene { get; private set; }
    public static TempSceneRefs scene = new TempSceneRefs();

    [SerializeField] PlayerManager player_manager;
    [SerializeField] AudioManager audio_manager;
    [SerializeField] LoadoutFactory loadout_factory;
    [SerializeField] GameObject end_game_canvas;

    private static GameManager instance;


    void Awake()
    {
        if (instance == null)
        {
            InitSingleton();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    void InitSingleton()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }


    void Update()
    {
        if (round_over)
        {
            round_over = false;
            StartCoroutine(EndOfRound());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(1);
    }


    IEnumerator EndOfRound()
    {
        Time.timeScale = 0.3f;
        restarting_scene = true;

        yield return new WaitForSecondsRealtime(3);

        Time.timeScale = 1;
        end_game_canvas.SetActive(true);

        yield return new WaitForSecondsRealtime(3);

        restarting_scene = false;
        end_game_canvas.SetActive(false);

        PlayerManager.IdleAllPlayers();
        SceneManager.LoadScene(0);
    }


    void OnLevelWasLoaded(int _level)
    {
        PlayerManager.IdleAllPlayers();
    }

}
