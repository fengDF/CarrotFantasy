using UnityEngine;

/// <summary>
/// 负责控制游戏中音乐音效的播放与停止
/// </summary>
public class AudioManager
{
    private AudioSource[] audioSources; // 0:播放背景音乐 1:播放音效
    public bool playBGMusic = true; // 是否播放背景音乐
    public bool playEffectMusic = true; // 是否播放音效

    public AudioManager()
    {
        audioSources = GameManager.Instance.GetComponents<AudioSource>();
    }

    // 播放背景音乐
    public void PlayBGMusic(AudioClip clip,float volume = 0.5f)
    {
        if (!audioSources[0].isPlaying || audioSources[0].clip != clip)
        {
            audioSources[0].clip = clip;
            audioSources[0].volume = volume;
            audioSources[0].Play();
        }
    }

    // 播放音效
    public void PlayEffectMusic(AudioClip clip,float volume = 0.5f)
    {
        if (playEffectMusic)
        {
            audioSources[1].PlayOneShot(clip,volume);
        }
    }

    // 关闭背景音乐的方法
    public void CloseBGMusic()
    {
        audioSources[0].Stop();
    }

    // 开启背景音乐的方法
    public void OpenBGMusic()
    {
        audioSources[0].Play();
    }

    // 当音乐控制开关按下时
    public void BGMButtonClick()
    {
        playBGMusic = !playBGMusic;
        if (playBGMusic)
        {
            OpenBGMusic();
        }
        else
        {
            CloseBGMusic();
        }
    }

    // 当音效控制开关按下时
    public void EffectMusicButtonClick()
    {
        playEffectMusic = !playEffectMusic;
    }

    // 播放按钮点击的音效
    public void PlayButtonAudioEffect()
    {
        PlayEffectMusic(GameManager.Instance.GetAudioClip("Main/Button"),0.35f);
    }

    // 播放翻书音效
    public void PlayPageAudioEffect()
    {
        PlayEffectMusic(GameManager.Instance.GetAudioClip("Main/Paging"),0.25f);
    }
}
