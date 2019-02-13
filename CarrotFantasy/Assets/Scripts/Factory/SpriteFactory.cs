using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图片资源工厂
/// </summary>
public class SpriteFactory : IBaseResourceFactory<Sprite>
{
    protected Dictionary<string, Sprite> factoryDict = new Dictionary<string, Sprite>(); // 存储图片资源的工厂
    protected string loadPath; // 图片资源加载路径前缀

    public SpriteFactory()
    {
        loadPath = "Pictures/";
    }

    // 工厂生产需要的图片资源
    public Sprite GetSingleResources(string resourcePath)
    {
        Sprite itemGO = null;
        string path = loadPath + resourcePath; // 需要的图片资源所在的完整路径
        if (factoryDict.ContainsKey(resourcePath)) // 如果工厂中有该资源直接返回
        {
            itemGO = factoryDict[resourcePath];
        }
        else // 没有则加载并存入工厂字典
        {
            itemGO = Resources.Load<Sprite>(path);
            factoryDict.Add(resourcePath, itemGO);
        }

        if (itemGO == null) // 资源加载异常处理
        {
            Debug.LogError(string.Format("资源加载异常: {0}资源加载失败,加载路径为{1}", resourcePath, path));
        }

        return itemGO;
    }
}
