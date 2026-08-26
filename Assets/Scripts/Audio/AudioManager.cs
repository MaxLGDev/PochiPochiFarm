using System;
using UnityEngine;
using UnityEngine.Audio;

// BGM・効果音・音量管理を行うクラス
public class SoundManager : MonoBehaviour
{
    // シングルトン参照
    public static SoundManager Instance;

    [SerializeField] private AudioMixer mixer;

    // AudioMixer のパラメータ名
    private const string MASTER_VOLUME = "MasterVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string SFX_VOLUME = "SFXVolume";

    // PlayerPrefs 保存キー
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";

    // 音データ
    public Sound[] musicSounds, effectSounds;

    // 再生用 AudioSource
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private bool isMuted;

    // シングルトン初期化
    private void Awake()
    {
        if (mixer == null)
            Debug.LogError("No mixer assigned", this);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーン跨ぎで保持
        }
        else
        {
            Destroy(gameObject); // 重複防止
        }
    }

    private void Start()
    {
        // 保存済み音量の読み込み（デフォルト0.5）
        float music = PlayerPrefs.GetFloat(MUSIC_PREF, 0.5f);
        float sfx = PlayerPrefs.GetFloat(SFX_PREF, 0.5f);

        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    // 共通再生処理
    private void PlaySound(
        Sound[] soundArray,
        AudioSource source,
        string name)
    {
        if (source == null)
        {
            Debug.LogWarning("AudioSource が設定されていません");
            return;
        }

        // 名前で検索
        Sound sound = Array.Find(soundArray, s => s.soundName == name);

        if (sound == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }
        
        source.clip = sound.audioClip;
        source.Play();
    }

    // BGM再生（既存停止）
    public void PlayMusic(string name)
    {
        if (musicSource == null)
        {
            Debug.LogError("Music Source is missing");
            return;
        }
        
        if (musicSource.isPlaying)
            musicSource.Stop();

        PlaySound(musicSounds, musicSource, name);
    }

    // 全体一時停止
    public void PauseAll()
    {
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError("Music Source and/or SFX Source are missing");
            return;
        }
        
        if (musicSource.isPlaying)
            musicSource.Pause();

        if (sfxSource.isPlaying)
            sfxSource.Pause();
    }

    // 再開
    public void ResumeAll()
    {
        if (musicSource == null || sfxSource == null)
        {
            Debug.LogError("Music Source and/or SFX Source are missing");
            return;
        }
        
        if (!musicSource.isPlaying && musicSource.clip != null)
            musicSource.UnPause();

        if (!sfxSource.isPlaying && sfxSource.clip != null)
            sfxSource.UnPause();
    }

    // 効果音再生（ピッチ変更可）
    public void PlaySFX(string name, float pitch = 1f)
    {
        if(sfxSource == null)
        {
            Debug.LogError("SFX Source is missing");
            return;
        }
        
        Sound sfxSound =
            Array.Find(effectSounds, s => s.soundName == name);

        if (sfxSound == null)
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(sfxSound.audioClip);
        sfxSource.pitch = 1f; // 元に戻す
    }

    // ボタン用ラッパー
    public void PlaySFX(string name)
    {
        PlaySFX(name, 1f);
    }

    // 0～1の値をdBへ変換して適用
    private void SetVolume(string parameter, float value)
    {
        value = Mathf.Clamp01(value);

        float db = value == 0 ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(parameter, db);
    }

    // BGM音量設定
    public void SetMusicVolume(float value)
    {
        SetVolume(MUSIC_VOLUME, value);
        PlayerPrefs.SetFloat(MUSIC_PREF, value);
    }

    // 効果音音量設定
    public void SetSFXVolume(float value)
    {
        SetVolume(SFX_VOLUME, value);
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }

    // ミュート切替
    public void ToggleMute() => SetMuted(!isMuted);

    public void SetMuted(bool mute)
    {
        isMuted = mute;
        mixer.SetFloat(MASTER_VOLUME, mute ? -80f : 0f);
    }

    public bool IsMuted() => isMuted;

    // アプリ一時停止時に保存
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            PlayerPrefs.Save();
    }
}