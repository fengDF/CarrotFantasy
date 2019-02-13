using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音频资源工厂
/// </summary>
public class AudioClipFactory : IBaseResourceFactory<AudioClip>
{
    protected Dictionary<string, AudioClip> factoryDict = new Dictionary<string, AudioClip>(); // 存储音频资源的工厂
    protected string loadPath; // 音频资源加载路径前缀

    public AudioClipFactory()
    {
        loadPath = "AudioClips/";
    }

    // 工厂生产需要的音频资源
    public AudioClip GetSingleResources(string resourcePath)
    {
        AudioClip itemGO = null;
        string path = loadPath + resourcePath; // 需要的音频资源所在的完整路径
        if (factoryDict.ContainsKey(resourcePath)) // 如果工厂中有该资源直接返回
        {
            itemGO = factoryDict[resourcePath];
        }
        else // 没有则加载并存入工厂字典
        {
            itemGO = Resources.Load<AudioClip>(path);
            factoryDict.Add(resourcePath, itemGO);
        }

        if (itemGO == null) // 资源加载异常处理
        {
            Debug.LogError(string.Format("资源加载异常: {0}资源加载失败,加载路径为{1}", resourcePath, path));
        }

        return itemGO;
    }
}
