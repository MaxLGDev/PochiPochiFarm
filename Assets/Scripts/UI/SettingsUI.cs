using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    public void ToggleSettingsPanel() => settingsPanel.SetActive(!settingsPanel.activeSelf);

    private void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
    }
}
