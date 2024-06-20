using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioEquilizerScript : MonoBehaviour
{
    public Slider _musicSlider;
    public Slider _sfxSlider;

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.instance.ToggleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(_sfxSlider.value);
    }

    private void Start()
    {
        if(AudioManager.instance != null)
        {
            _musicSlider.value = AudioManager.instance.StartMusicVolume;
            
        }
        else
        {
            Debug.LogError("AudioManager not found in the main menu scene.");
        }

    }
    private void Update()
    {
        
        MusicVolume();
        SFXVolume();

    }

}
