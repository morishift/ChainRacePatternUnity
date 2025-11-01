using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// サウンドタイプ
/// </summary>
public enum SoundType
{ 
    Gao1,
    Pong1,
    Pong2,
    Pong3,
    Pong4
}

/// <summary>
/// サウンド再生用コンポーネント
/// </summary>
public class SoundPlayer : MonoBehaviour
{
    [SerializeField] 
    AudioClip[] audioClips;
    List<AudioSource> audioSources = new List<AudioSource>();

    static SoundPlayer instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < 3; ++i)
        { 
            audioSources.Add(gameObject.AddComponent<AudioSource>());
        }
    }

    /// <summary>
    /// サウンドの再生
    /// </summary>
    public void PlaySound(SoundType sound)
    {
        int index = (int)sound;
        if (index >= audioClips.Length)
        {
            return;
        }
        PlayAudioClip(audioClips[(int)sound]);
    }

    /// <summary>
    /// 指定されたオーディオクリップを再生する
    /// </summary>
    public void PlayAudioClip(AudioClip clip)
    {
        int index = audioSources.FindIndex(_s => !_s.isPlaying);
        if (index < 0)
        {
            Debug.Log("Unknown");
            index = 0;
        }
        var source = audioSources[index];
        audioSources.RemoveAt(index);
        audioSources.Add(source);
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// SoundPlayerの取得
    /// </summary>
    public static SoundPlayer Get()
    {
        return instance;
    }
}


