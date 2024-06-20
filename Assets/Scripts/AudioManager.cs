using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; 

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private float SFXvolumeScale = 1.0F;

    private float _StartMusicVolume = 0.5f;

    [SerializeField]
    private float _CurrentMusicVolume = 0.5f; 
    public float CurrentMusicVolume { get => _CurrentMusicVolume; set => _CurrentMusicVolume = value; }

    public float StartMusicVolume
    {
        get => _StartMusicVolume;
        set => _StartMusicVolume = value;
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        

    }

    private void Start()
    {
        Debug.Log("AudioManager Start called");
       //PlayMusic("Theme");
        //PlaySFX("Win-Flute");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            Debug.Log("Playing Music");
            musicSource.clip = s.clip;
            musicSource.Play();  // This ensures the music actually plays
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip, SFXvolumeScale);
            sfxSource.Play();
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
      //Debug.Log("Music Volume: " + volume);
   _CurrentMusicVolume = volume;


}

public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}

