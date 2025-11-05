using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    [Header("---------- Audio Mixer ----------")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("---------- Sliders ----------")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    private void Start()
    {
        //Checks if 
        if(PlayerPrefs.HasKey("masterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMasterVolume();
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    //Sets master volume, changes all audio
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        masterMixer.SetFloat("master", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    //Sets music volume
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        masterMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    //Sets SFX volume
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        masterMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    //Loads volume that was saved last session
    private void LoadVolume()
    {
        if (masterSlider != null)
            masterSlider.value = PlayerPrefs.GetFloat("masterVolume");

        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");

        if (SFXSlider != null)
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        if (masterSlider != null) SetMasterVolume();
        if (musicSlider != null) SetMusicVolume();
        if (SFXSlider != null) SetSFXVolume();
    }
}
