using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private string githubUrl;
    [SerializeField] private string unityroomUrl;
    [SerializeField] private Toggle fullScreenToggle;

    private void Start()
    {
        SoundManager.Instance.PlayMusic("MainSong");
        fullScreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    public void OpenGithubLink() => Application.OpenURL(githubUrl);
    
    public void OpenUnityRoomLink() => Application.OpenURL(unityroomUrl);

    public void SetFullScreenTo(bool isOn) => Screen.fullScreen = isOn;

    public void CloseGame() => Application.Quit();
}
