using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audio_source;
    [SerializeField] List<AudioClip> audio_clips;


    void Start()
    {

    }


    public void PlayOneShot(string _clip_name)
    {
        AudioClip clip = GetAudioClip(_clip_name);

        if (clip != null)
            audio_source.PlayOneShot(clip);
    }


    public void PlayOneShot(AudioClip _clip)
    {
        if (_clip != null)
            audio_source.PlayOneShot(_clip);
    }


    public AudioClip GetAudioClip(string _clip_name)
    {
        return audio_clips.Find(item => item.name.Substring(0) == _clip_name);
    }

}
