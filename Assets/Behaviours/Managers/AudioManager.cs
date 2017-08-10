using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Range(0,1)][SerializeField] float music_volume = 1;
    [Range(0,1)][SerializeField] float sfx_volume = 1;
    [SerializeField] List<AudioClip> audio_clips;

    private AudioSource music_source;
    private AudioSource sfx_source;


    void Start()
    {
        GameObject audio_parent = new GameObject("Audio");
        audio_parent.transform.SetParent(this.transform);

        music_source = audio_parent.AddComponent<AudioSource>();
        sfx_source = audio_parent.AddComponent<AudioSource>();
    }


    public void PlayOneShot(string _clip_name)
    {
        AudioClip clip = GetAudioClip(_clip_name);

        if (clip != null)
            sfx_source.PlayOneShot(clip);
    }


    public void PlayOneShot(AudioClip _clip)
    {
        if (_clip != null)
            sfx_source.PlayOneShot(_clip);
    }


    public AudioClip GetAudioClip(string _clip_name)
    {
        return audio_clips.Find(item => item.name.Substring(0) == _clip_name);
    }

    
    public void Update()
    {
        music_source.volume = music_volume;
        sfx_source.volume = sfx_volume;
    }

}
