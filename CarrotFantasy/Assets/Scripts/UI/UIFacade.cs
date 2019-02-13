using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// UIPanel之间的中介,以及Manager与panel之间的外观角色
/// </summary>
public class UIFacade
{
    // 管理者
    private readonly UIManager mUIManager;
    private readonly GameManager mGameManager;
    private readonly AudioManager mAudioManager;
    private readonly PlayerManager mPlayerManager;

    //UI面板(当前场景状态下的UIPanel)
    public Dictionary<string, IBasePanel> currentScenePanelDict = new Dictionary<string, IBasePanel>();

    // Panel之间切换的Mask
    private GameObject mask;
    private Image maskImage;

    // 场景状态
    public IBaseSceneState currentSceneState;
    public IBaseSceneState lastSceneState;

    public Transform canvas; // UIPanel放置的容器

    public UIFacade(UIManager uIManager)
    {
        mUIManager = uIManager;
        mGameManager = GameManager.Instance;
        mAudioManager = mGameManager.AudioManager;
        mPlayerManager = mGameManager.PlayerManager;
        InitMask();
    }

    #region 场景切换状态的方法以及动画
    // 初始化遮罩
    public void InitMask()
    {
        canvas = GameObject.Find("Canvas").transform;
        mask = CreateUI("Img_Mask", canvas);
        maskImage = mask.GetComponent<Image>();
    }

    // 改变当前场景的状态
    public void ChangeSceneState(IBaseSceneState baseSceneState)
    {
        lastSceneState = currentSceneState;
        ShowMask();
        currentSceneState = baseSceneState;
    }

    // 显示遮罩(动画)
    private void ShowMask()
    {
        mask.transform.SetSiblingIndex(10);
        Tween anim = DOTween.To(() => maskImage.color, toColor => maskImage.color = toColor, new Color(0, 0, 0, 1), 2f);
        anim.OnComplete(ExitScene);
    }

    // 离开当前场景的方法
    private void ExitScene()
    {
        lastSceneState.ExitScene();
        currentSceneState.EnterScene();
        HideMask();
    }

    // 隐藏遮罩(动画)
    private void HideMask()
    {
        mask.transform.SetSiblingIndex(10);
        DOTween.To(() => maskImage.color, toColor => maskImage.color = toColor, new Color(0, 0, 0, 0), 2f);
    }
    #endregion

    // 将UIPanel添加进UIManager字典
    public void AddPanelToDict(string uIPanelName)
    {
        mUIManager.currentScenePanelDict.Add(uIPanelName, GetItem(FactoryType.UIPanel, uIPanelName));
    }

    // 实例化当前场景下的UIPanel并存入字典
    public void InitUIPanelDict()
    {
        foreach (var item in mUIManager.currentScenePanelDict)
        {
            item.Value.transform.SetParent(canvas);
            item.Value.transform.localPosition = Vector3.zero;
            item.Value.transform.localScale = Vector3.one;
            IBasePanel basePanel = item.Value.GetComponent<IBasePanel>();
            if (basePanel == null)
            {
                Debug.LogWarning(string.Format("{0}上的IBasePanel脚本丢失!", item.Key));
            }
            basePanel.InitPanel();  // UIPanel初始化UI
            currentScenePanelDict.Add(item.Key, basePanel); // 将该场景下的UIPanel身上的Panel脚本添加进字典中
        }
    }

    // 清空UIPanel字典
    public void ClearUIPanelDict()
    {
        currentScenePanelDict.Clear();
        mUIManager.ClearUIPanelDict();
    }

    // 实例化UI
    public GameObject CreateUI(string uIName, Transform parent)
    {
        GameObject itemGO = GetItem(FactoryType.UI, uIName);
        itemGO.transform.SetParent(parent);
        itemGO.transform.localPosition = Vector3.zero;
        itemGO.transform.localScale = Vector3.one;
        return itemGO;
    }

    #region 通过UIFacade向PlayerManager发送数据

    // 发送保存玩家数据的方法
    public void SaveData()
    {
        mPlayerManager.SaveData();
    }

    // 设置关卡结束后的统计数据
    public void SetStatisticData(int coin, int clearItems, int killMonsters)
    {
        mPlayerManager.MoneyNum += coin;
        mPlayerManager.ClearItemCount = clearItems;
        mPlayerManager.ClearMonsterCount = killMonsters;
    }

    // 设置通过关卡的统计数据
    public void SetNormalNum(int num = 1)
    {
        mPlayerManager.NormalNum += num;
    }

    // 修改PlayerManager的怪物窝数据
    public void SetMonsterData(int cookies, int milks, int nest)
    {
        mPlayerManager.CookiesCount += cookies;
        mPlayerManager.MilkCount += milks;
        mPlayerManager.NestCount += nest;
    }

    public void BuyMonsterItem(int price)
    {
        mPlayerManager.BuyMonsterItem(price);
    }

    // 解锁关卡的数据设置
    public void SetLevelData(int index)
    {
        if (index <= 3)
        {
            mPlayerManager.UnLockedLevelNum[0] = index + 1;
            GetLevelStage(index).unLocked = true;
        }
        else if (index == 4) // 代表通关除隐藏关卡的最后一关
        {
            mPlayerManager.UnLockedBigLevelList[1] = true; // 解锁第二个主题
            GetLevelStage(index+1).unLocked = true; // 解锁第二个主题的第一关
            mPlayerManager.UnLockedLevelNum[1] = 1;
        }
        else if (index <= 8)
        {
            mPlayerManager.UnLockedLevelNum[1] = index - 4;
            GetLevelStage(index).unLocked = true;
        }
        else if (index == 9)
        {
            mPlayerManager.UnLockedBigLevelList[2] = true; // 解锁第二个主题
            GetLevelStage(index + 1).unLocked = true; // 解锁第二个主题的第一关
            mPlayerManager.UnLockedLevelNum[2] = 1;
        }
        else if (index <= 13)
        {
            mPlayerManager.UnLockedLevelNum[2] = index - 9;
            GetLevelStage(index).unLocked = true;
        }

    }

    // 增加获得的宠物蛋信息
    public void SetMonsterEgg(MonsterPetData monsterPetData)
    {
        mPlayerManager.MonsterPetDataList.Add(monsterPetData);
    }
    #endregion

    #region 通过UIFacade向GameManager发送数据(外观者)

    public void SetCurrentStage(int index)
    {
        mGameManager.CurrentStage = GetLevelStage(index);
    }

    public void ResetGame()
    {
        mGameManager.initPlayerManager = true;
        mPlayerManager.LoadData();
    }

    #endregion

    #region 通过UIFacade向PlayerManager获取数据并传递给下层使用(外观者)

    // 向PlayerManager获取统计的数据
    public int[] GetStatisticsData()
    {
        int[] data = new int[7];
        data[0] = mPlayerManager.NormalNum;
        data[1] = mPlayerManager.HideLevelNum;
        data[2] = mPlayerManager.BossNum;
        data[3] = mPlayerManager.MoneyNum;
        data[4] = mPlayerManager.ClearMonsterCount;
        data[5] = mPlayerManager.ClearBossCount;
        data[6] = mPlayerManager.ClearItemCount;

        return data;
    }

    // 向PlayerManager获取大关卡游戏信息
    public BigLevelData GetBigLevelData(int index)
    {
        BigLevelData bigLevelData = new BigLevelData()
        {
            unLocked = mPlayerManager.UnLockedBigLevelList[index],
            lockedLevelNum = mPlayerManager.UnLockedLevelNum[index],
            totalLevelNum = 5
        };
        return bigLevelData;
    }

    public Stage GetLevelStage(int index)
    {
        return mPlayerManager.LevelStageList[index];
    }

    // 向PlayerManager获取怪物窝信息
    public List<MonsterPetData> GetMonsterPetData()
    {
        return mPlayerManager.MonsterPetDataList;
    }

    // 向PlayerManager获取怪物窝道具的信息
    public int[] GetMonsterPetItem()
    {
        int[] data = new int[4];
        data[0] = mPlayerManager.CookiesCount;
        data[1] = mPlayerManager.MilkCount;
        data[2] = mPlayerManager.NestCount;
        data[3] = mPlayerManager.Diamands;
        return data;
    }

    #endregion

    #region 通过UIFacade向音乐控制器发送开关音乐的事件(外观者)

    public void BGMMusicButtonClick()
    {
        mAudioManager.BGMButtonClick();
    }

    public void EffectMusicButtonClick()
    {
        mAudioManager.EffectMusicButtonClick();
    }

    // 按钮声音
    public void PlayButtonAudioEffect()
    {
        mAudioManager.PlayButtonAudioEffect();
    }

    //翻书声音
    public void PlayPageAudioEffect()
    {
        mAudioManager.PlayPageAudioEffect();
    }

    #endregion

    #region 通过GameManager向工厂获取资源的系列方法(中介者)

    // 获取Sprite资源的方法
    public Sprite GetSprite(string resourcesPath)
    {
        return mGameManager.GetSprite(resourcesPath);
    }

    // 获取AudioClip资源的方法
    public AudioClip GetAudioClip(string resourcesPath)
    {
        return mGameManager.GetAudioClip(resourcesPath);
    }

    // 获取RuntimeAnimator资源的方法
    public RuntimeAnimatorController GetRuntimeAnimator(string resourcesPath)
    {
        return mGameManager.GetRuntimeAnimator(resourcesPath);
    }

    // 获取游戏物体的方法
    public GameObject GetItem(FactoryType factoryType, string itemName)
    {
        return mGameManager.GetItem(factoryType, itemName);
    }

    // 将游戏物体放回对象池的方法
    public void PushItem(FactoryType factoryType, string itemName, GameObject item)
    {
        mGameManager.PushItem(factoryType, itemName, item);
    }

    #endregion
}

// 存储大关卡游戏信息
public struct BigLevelData
{
    public bool unLocked;
    public int lockedLevelNum;
    public int totalLevelNum;
}
