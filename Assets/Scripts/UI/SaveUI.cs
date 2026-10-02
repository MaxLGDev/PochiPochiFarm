using UnityEngine;

public class SaveUI : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private GameObject resetConfirmationPanel;

    public void ToggleResetConfirmationPanel() => resetConfirmationPanel.SetActive(!resetConfirmationPanel.activeSelf);

    public void ConfirmReset() => saveManager.ResetSave();
}
