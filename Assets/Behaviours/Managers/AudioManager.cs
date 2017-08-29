using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private bool sfx_delay;


    public static void PlayOneShot(string _clip_name)
    {
        PlayOneShot(instance.GetAudioClip(_clip_name));
    }


    public static void PlayOneShot(AudioClip _clip)
    {
        if (instance.sfx_delay)
            return;

        instance.sfx_delay = true;

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

        if (music != null)
        {
            music_source.clip = music;
            music_source.loop = true;

            music_source.Play();
        }
    }


    void Update()
    {
        music_source.volume = music_volume_;
        sfx_source.volume = sfx_volume_;

        sfx_delay = false;
    }

}
