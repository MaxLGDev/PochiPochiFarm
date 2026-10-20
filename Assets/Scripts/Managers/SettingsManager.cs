using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;
using System;
using UnityEngine.Localization;
using UnityEngine.Rendering;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private string githubUrl;
    [SerializeField] private string unityroomUrl;
    [SerializeField] private Toggle fullScreenToggle;

    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;
    [SerializeField] private TMP_Text enLocaleSettingsText;
    [SerializeField] private TMP_Text jpLocaleSettingsText;
    [SerializeField] private TMP_Text enLocaleTitleText;
    [SerializeField] private TMP_Text jpLocaleTitleText;

    private Vector2Int windowedSize = new Vector2Int(1280, 720);
    private bool lastFullScreen;

    private Action<Locale> onLocaleChanged;

    private void Awake()
    {
        onLocaleChanged = UpdateLocaleColors;
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += onLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= onLocaleChanged;
    }

    private void Start()
    {
        SoundManager.Instance.PlayMusic("MainSong");
        fullScreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

        lastFullScreen = Screen.fullScreen;
        
        // Only remember the current size if we're actually windowed
        if (!Screen.fullScreen)
            windowedSize = new Vector2Int(Screen.width, Screen.height);

        LocalizationSettings.SelectedLocaleAsync.Completed += handle => UpdateLocaleColors(handle.Result);
    }

    private void Update()
    {
        if (Screen.fullScreen == lastFullScreen)
            return;
        
        lastFullScreen = Screen.fullScreen;
        fullScreenToggle.SetIsOnWithoutNotify(lastFullScreen);
    }

    public void SetLanguage(int index)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;
        if (index < 0 || index >= locales.Count)
            return;

        LocalizationSettings.SelectedLocale = locales[index];
    }

    private void UpdateLocaleColors(Locale locale)
    {
        bool english = locale.Identifier.Code.StartsWith("en");

        Color en = english ? activeColor : inactiveColor;
        Color jp = english ? inactiveColor : activeColor;

        enLocaleSettingsText.color = en;
        jpLocaleSettingsText.color = jp;
        enLocaleTitleText.color = en;
        jpLocaleTitleText.color = jp;
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
