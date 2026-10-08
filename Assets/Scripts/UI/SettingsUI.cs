using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    public void ToggleSettingsPanel()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        SoundManager.Instance.PlaySFX("TogglePanel");
    }

    public void OnMusicSliderChanged(float value) => SoundManager.Instance.SetMusicVolume(value);
    public void OnSFXSliderChanged(float value) => SoundManager.Instance.SetSFXVolume(value);

    private void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        OnMusicSliderChanged(music);
        OnSFXSliderChanged(sfx);
    }
}
