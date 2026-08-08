using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if(musicSlider != null)
        {
            float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.SetValueWithoutNotify(savedMusicVolume);
        }

        if(sfxSlider != null)
        {
            float savedSFXVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sfxSlider.SetValueWithoutNotify(savedSFXVolume);
        }

    }

    public void ChangeMusicVolume(float volume)
    {
        if(AudioManager.instance != null)
        {             
        AudioManager.instance.SetMusicVolume(volume);
        }
    }

    // Update is called once per frame
    public void ChangeSFXVolume(float volume)
    {
        if(AudioManager.instance != null)
        {
            AudioManager.instance.SetSFXVolume(volume);
        }

    }
}
