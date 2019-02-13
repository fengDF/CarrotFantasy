using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画控制器资源工厂
/// </summary>
public class RuntimeAnimatorFactory : IBaseResourceFactory<RuntimeAnimatorController>
{
    protected Dictionary<string, RuntimeAnimatorController> factoryDict = new Dictionary<string, RuntimeAnimatorController>(); // 存储动画控制器资源的工厂
    protected string loadPath; // 动画控制器加载路径前缀

    public RuntimeAnimatorFactory()
    {
        loadPath = "Animator/AnimatorController/";
    }

    // 工厂生产需要的动画控制器资源
    public RuntimeAnimatorController GetSingleResources(string resourcePath)
    {
        RuntimeAnimatorController itemGO = null;
        string path = loadPath + resourcePath; // 需要的动画控制器资源所在的完整路径
        if (factoryDict.ContainsKey(resourcePath)) // 如果工厂中有该资源直接返回
        {
            itemGO = factoryDict[resourcePath];
        }
        else // 没有则加载并存入工厂字典
        {
            itemGO = Resources.Load<RuntimeAnimatorController>(path);
            factoryDict.Add(resourcePath, itemGO);
        }

        if (itemGO == null) // 资源加载异常处理
        {
            Debug.LogError(string.Format("资源加载异常: {0}资源加载失败,加载路径为{1}", resourcePath, path));
        }

        return itemGO;
    }
}
