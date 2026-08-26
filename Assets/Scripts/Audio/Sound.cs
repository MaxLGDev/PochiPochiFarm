using UnityEngine;

// 音データをまとめるシンプルな構造体クラス
[System.Serializable]
public class Sound
{
    // サウンド識別用の名前（検索キー）
    public string soundName;

    // 再生する AudioClip
    public AudioClip audioClip;
}