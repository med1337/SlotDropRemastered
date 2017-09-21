using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class AudioSettings
{
    public double music_volume;
    public double sfx_volume;
}

public class AudioManager : MonoBehaviour
{
    public static float music_volume { get { return instance.music_volume_; } set { instance.music_volume_ = value; } }
    public static float sfx_volume { get { return instance.sfx_volume_; } set { instance.sfx_volume_ = value; } }

    [Range(0, 1)][SerializeField] float music_volume_ = 1;
    [Range(0, 1)][SerializeField] float sfx_volume_ = 1;

    [SerializeField] AudioClip music;
    [SerializeField] List<AudioClip> audio_clips;

    private static AudioManager instance;

    private AudioSource music_source;
    private AudioSource sfx_source;
    private AudioClip last_clip_played;


    public static void PlayOneShot(string _clip_name)
    {
        PlayOneShot(instance.GetAudioClip(_clip_name));
    }


    public static void PlayOneShot(AudioClip _clip)
    {
        if (instance.last_clip_played == _clip)
            return;

        instance.last_clip_played = _clip;

        if (_clip != null)
            instance.sfx_source.PlayOneShot(_clip);
    }


    public static void StopAllSFX()
    {
        instance.sfx_source.Stop();
    }


    public AudioClip GetAudioClip(string _clip_name)
    {
        return audio_clips.Find(item => item.name.Substring(0) == _clip_name);
    }


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

        GameObject audio_parent = new GameObject("Audio");
        audio_parent.transform.SetParent(this.transform);

        music_source = audio_parent.AddComponent<AudioSource>();
        sfx_source = audio_parent.AddComponent<AudioSource>();

        ReadJSONSettings();

        if (music != null)
        {
            music_source.clip = music;
            music_source.loop = true;

            music_source.Play();
        }
    }


    void ReadJSONSettings()
    {
        AudioSettings settings = new AudioSettings();

        if (File.Exists(Application.streamingAssetsPath + "/settings.json"))
        {
            // Load existing settings file.
            JsonData settings_json = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/settings.json"));

            settings.music_volume = double.Parse(settings_json["music_volume"].ToString());
            settings.sfx_volume = double.Parse(settings_json["sfx_volume"].ToString());
        }
        else
        {
            // Generate a new settings file.
            settings.music_volume = 0.25f;
            settings.sfx_volume = 0.5f;

            SaveSettings(settings);
        }

        music_volume = (float)settings.music_volume;
        sfx_volume = (float)settings.sfx_volume;
    }


    void Update()
    {
        audio_clips.RemoveAll(elem => elem == null);

        music_source.volume = music_volume_;
        sfx_source.volume = sfx_volume_;

        last_clip_played = null;
    }


    void OnApplicationQuit()
    {
        AudioSettings settings = new AudioSettings();

        settings.music_volume = music_volume;
        settings.sfx_volume = sfx_volume;

        SaveSettings(settings);
    }


    void SaveSettings(AudioSettings _settings)
    {
        JsonData settings_json = JsonMapper.ToJson(_settings);
        File.WriteAllText(Application.streamingAssetsPath + "/settings.json", settings_json.ToString());
    }

}
