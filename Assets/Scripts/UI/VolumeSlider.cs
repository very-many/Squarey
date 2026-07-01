using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmallHedge.SoundManager;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider effectSlider;
    [SerializeField] Slider musicSlider;

    private const string prefKeyEffectVolume = "EffectVolume";
    private const string prefKeyMusicVolume = "MusicVolume";

    void Start()
    {
        if (!PlayerPrefs.HasKey(prefKeyEffectVolume))
            PlayerPrefs.SetFloat(prefKeyEffectVolume, 1f);

        if (!PlayerPrefs.HasKey(prefKeyMusicVolume))
            PlayerPrefs.SetFloat(prefKeyMusicVolume, 1f);

        LoadVolumeSliders();
        SetEffectVolume(effectSlider.value);
        SetMusicVolume(musicSlider.value);
    }

    public void SetEffectVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat(prefKeyEffectVolume, Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat(prefKeyMusicVolume, Mathf.Log10(volume) * 20);
    }

    private void LoadVolumeSliders()
    {
        effectSlider.value = PlayerPrefs.GetFloat(prefKeyEffectVolume, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(prefKeyMusicVolume, 1f);
    }

    public void SaveEffectVolumeSlider()
    {
        PlayerPrefs.SetFloat(prefKeyEffectVolume, effectSlider.value);
    }
    public void SaveMusicVolumeSlider()
    {
        PlayerPrefs.SetFloat(prefKeyMusicVolume, musicSlider.value);
    }
}