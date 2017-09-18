using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool restarting_scene;
    public static TempSceneRefs scene = new TempSceneRefs();
    public static bool cheats_enabled { get; set; }

    [SerializeField] PlayerManager player_manager;
    [SerializeField] AudioManager audio_manager;
    [SerializeField] LoadoutFactory loadout_factory;
    public GameObject end_game_canvas;
    [SerializeField] private bool cheats_enabled_ = false;

    public static GameManager instance;


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
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.StopAllSFX();
            SceneManager.LoadScene(0);
        }

        cheats_enabled = cheats_enabled_;
    }


    void OnLevelWasLoaded(int _level)
    {
        PlayerManager.IdleAllPlayers();
    }

}
