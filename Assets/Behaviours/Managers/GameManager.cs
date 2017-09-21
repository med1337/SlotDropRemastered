using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool restarting_scene;
    public static TempSceneRefs scene = new TempSceneRefs();
    public static bool cheats_enabled;
    public static float session_start { get; private set; }
    public static int min_ai;

    [SerializeField] PlayerManager player_manager;
    [SerializeField] AudioManager audio_manager;
    [SerializeField] LoadoutFactory loadout_factory;

    private static GameManager instance;
    private const int MAX_AI = 32;


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
        min_ai = Mathf.Clamp(min_ai, 0, MAX_AI);

        if (Input.GetKeyDown(KeyCode.C))
        {
            cheats_enabled = !cheats_enabled;
            AudioManager.PlayOneShot("wink");

            if (scene.general_canvas_manager != null)
                scene.general_canvas_manager.FlashCheatsPrompt(1);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.StopAllSFX();
            SceneManager.LoadScene(0);
        }
    }


    void OnLevelWasLoaded(int _level)
    {
        PlayerManager.IdleAllPlayers();
        session_start = Time.realtimeSinceStartup;
        cheats_enabled = false;
    }

}
