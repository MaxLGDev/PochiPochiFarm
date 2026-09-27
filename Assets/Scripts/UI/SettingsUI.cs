using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    
    public void ToggleSettingsPanel() => settingsPanel.SetActive(!settingsPanel.activeSelf);
}
