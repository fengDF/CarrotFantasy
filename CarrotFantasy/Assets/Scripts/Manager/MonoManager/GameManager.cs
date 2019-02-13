using UnityEngine;

/// <summary>
/// 游戏总管理,负责管理其他所有的管理者
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // GameManager单例

    // 其他管理者的单实例
    public PlayerManager PlayerManager { get; private set; }
    public FactoryManager FactoryManager { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public UIManager UIManager { get; private set; }

    public Stage CurrentStage { get; set; }
    public bool initPlayerManager; // 是否重置玩家数据(读取数据时读取初始文件)

    private void Awake()
    {
        Screen.SetResolution(1024, 768, false);
        DontDestroyOnLoad(gameObject);
        Instance = this; // 实例化单例
        //实例化其他管理者的单实例
        PlayerManager = new PlayerManager();
        // PlayerManager.SaveData(); 开发用于保存玩家初始数据Json
        PlayerManager.LoadData();
        FactoryManager = new FactoryManager();
        AudioManager = new AudioManager();
        UIManager = new UIManager();
        UIManager.mUIFacade.currentSceneState.EnterScene(); // 手动进入第一个场景状态
        CurrentStage = PlayerManager.LevelStageList[0];
    }

    private void OnDestroy()
    {
        PlayerManager.SaveData();
    }

    // 实例化游戏物体的方法
    public GameObject CreateItem(GameObject prefab)
    {
        return Instantiate(prefab);
    }

    #region 通过GameManager向工厂获取资源的系列方法(中介)

    // 获取Sprite资源的方法
    public Sprite GetSprite(string resourcesPath)
    {
        return FactoryManager.spriteFactory.GetSingleResources(resourcesPath);
    }

    // 获取AudioClip资源的方法
    public AudioClip GetAudioClip(string resourcesPath)
    {
        return FactoryManager.audioClipFactory.GetSingleResources(resourcesPath);
    }

    // 获取RuntimeAnimator资源的方法
    public RuntimeAnimatorController GetRuntimeAnimator(string resourcesPath)
    {
        return FactoryManager.runtimeAnimatorFactory.GetSingleResources(resourcesPath);
    }

    // 获取游戏物体的方法
    public GameObject GetItem(FactoryType factoryType, string itemName)
    {
        return FactoryManager.factoryDict[factoryType].GetItem(itemName);
    }

    // 将游戏物体放回对象池的方法
    public void PushItem(FactoryType factoryType, string itemName, GameObject item)
    {
        FactoryManager.factoryDict[factoryType].PushItem(itemName, item);
    }

    #endregion
}
