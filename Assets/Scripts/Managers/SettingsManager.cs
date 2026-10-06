using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private string githubUrl;
    [SerializeField] private string unityroomUrl;
    [SerializeField] private Toggle fullScreenToggle;

    private Vector2Int windowedSize = new Vector2Int(1280, 720);

    private void Start()
    {
        SoundManager.Instance.PlayMusic("MainSong");
        fullScreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
        
        // Only remember the current size if we're actually windowed
        if (!Screen.fullScreen)
            windowedSize = new Vector2Int(Screen.width, Screen.height);
    }

    public void OpenGithubLink() => Application.OpenURL(githubUrl);
    
    public void OpenUnityRoomLink() => Application.OpenURL(unityroomUrl);

    public void SetFullScreenTo(bool isOn)
    {
        if (isOn)
        {
            windowedSize = new Vector2Int(Screen.width, Screen.height);
            Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, FullScreenMode.FullScreenWindow);
        }
        else
            Screen.SetResolution(windowedSize.x, windowedSize.y, FullScreenMode.Windowed);
        
    }

    public void CloseGame() => Application.Quit();
}
