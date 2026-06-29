using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    [SerializeField] Slider effectSlider;
    [SerializeField] Slider musicSlider;

    void Start()
    {
        if(!PlayerPrefs.HasKey("EffectVolume")
        {
            PlayerPrefs.SetFloat("EffectVolume", 1f);
        }

        if(!PlayerPrefs.HasKey("MusicVolume")
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }

        else
        {
            LoadVolumeSlider();
        }
    }

    public void SetEffectVolume(float volume)
    {
        //AudioManager.Instance.SetEffectVolume(volume);
        AudioListener.volume = effectSlider.value;
        AudioListener.volume = musicSlider.value;

    }

    public void SetMusicVolume(float volume)
    {
        //AudioManager.Instance.SetMusicVolume(volume);
        AudioListener.volume = musicSlider.value;
        AudioListener.volume = effectSlider.value;
        SaveVolumeSlider(); //? Save only when back button is pressed, not every time the slider is changed?
    }

    private void LoadVolumeSlider()
    {
        effectSlider.value = PlayerPrefs.GetFloat("EffectVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    private void SaveVolumeSlider()
    {
        PlayerPrefs.SetFloat("EffectVolume", effectSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }
}
