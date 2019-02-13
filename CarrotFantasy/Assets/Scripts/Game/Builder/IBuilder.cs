using UnityEngine;

/// <summary>
/// 建造者基类,获得需要的对象
/// </summary>
/// <typeparam name="T">需要获取的类型</typeparam>
public interface IBuilder<T>
{
    /// <summary>
    /// 通过工厂获取需要的游戏对象
    /// </summary>
    GameObject GetProduct();
    /// <summary>
    /// 通过游戏物体获得身上的脚本
    /// </summary>
    T GetProductClass(GameObject gameObject);
    /// <summary>
    /// 获取脚本上的信息
    /// </summary>
    void GetData(T productClass);
    /// <summary>
    /// 获取其他资源
    /// </summary>
    void GetOtherResource(T productClass);
}
