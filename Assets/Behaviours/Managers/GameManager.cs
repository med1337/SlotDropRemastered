using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using LitJson;

public class GameManager : MonoBehaviour
{
    public static bool restarting_scene;
    public static TempSceneRefs scene = new TempSceneRefs();
    public static bool cheats_enabled;
    public static float session_start { get; private set; }
    public static int min_ai;
    public static GameSettings settings = new GameSettings();

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

        LoadGameSettings();
        DontDestroyOnLoad(this.gameObject);
    }


    void LoadGameSettings()
    {
        if (File.Exists(Application.streamingAssetsPath + "/settings.json"))
        {
            // Load existing settings file.
            JsonData settings_json = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/settings.json"));

            settings.music_volume = double.Parse(settings_json["music_volume"].ToString());
            settings.sfx_volume = double.Parse(settings_json["sfx_volume"].ToString());
            settings.starting_ai = int.Parse(settings_json["starting_ai"].ToString());
        }
        else
        {
            // Generate a new settings file.
            settings.music_volume = 0.25f;
            settings.sfx_volume = 0.5f;
            settings.starting_ai = 0;

            SaveSettings(settings);
        }

        min_ai = settings.starting_ai;
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
        min_ai = settings.starting_ai;
    }


    void OnApplicationQuit()
    {
        SaveSettings(settings);
    }


    void SaveSettings(GameSettings _settings)
    {
        JsonData settings_json = JsonMapper.ToJson(_settings);
        File.WriteAllText(Application.streamingAssetsPath + "/settings.json", settings_json.ToString());
    }

}
