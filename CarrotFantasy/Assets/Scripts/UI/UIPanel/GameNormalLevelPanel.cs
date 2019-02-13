using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 冒险模式小关卡选择面板
/// </summary>
public class GameNormalLevelPanel : BasePanel
{
    private int currentBigLevelID; // 当前的大关卡

    private string filePath; // 资源路径
    private int currentLevelID;
    private Transform levelContent; // 所有关卡的容器Content
    private GameObject lockedButtonImg; // 锁定按钮的遮罩图
    private Transform towerListEmp; //建塔列表
    // 左右环境背景图
    private Image leftIconImg;
    private Image rightIconImg;

    // 关卡状态显示的UI
    private Image carrotImg;
    private Image allClearImg;
    private Text waveText;

    private ScrollOneEffect scrollOneEffect;
    private List<GameObject> levelContentImgList; // 关卡列表
    private List<GameObject> towerContentImgList; // 建塔列表

    protected override void Awake()
    {
        base.Awake();

        // 所有变量和引用的初始化赋值
        filePath = "GameOption/Normal/Level/";
        levelContentImgList = new List<GameObject>();
        towerContentImgList = new List<GameObject>();

        levelContent = transform.Find("Scroll View").GetComponent<ScrollRect>().content;
        lockedButtonImg = transform.Find("Img_Lock").gameObject;
        towerListEmp = transform.Find("Emp_TowerList");
        leftIconImg = transform.Find("Img_LeftIcon").GetComponent<Image>();
        rightIconImg = transform.Find("Img_RightIcon").GetComponent<Image>();
        waveText = transform.Find("Img_TotalWaves/Txt_TotalWaves").GetComponent<Text>();
        scrollOneEffect = transform.Find("Scroll View").GetComponent<ScrollOneEffect>();
        currentLevelID = 1;

        LoadResources();
    }

    #region 面板进入退出的方法
    // 从大关卡进入小关卡选择面板的方法
    public void EnterPanelFromBigLevel(int bigLevelID)
    {
        currentBigLevelID = bigLevelID; // 赋值当前的主题
        EnterPanel();
    }

    public override void InitPanel()
    {
        gameObject.SetActive(false);
    }

    public override void EnterPanel()
    {
        gameObject.SetActive(true);
        currentLevelID = 1; // 从第一张地图开始
        string spritePath = filePath + currentBigLevelID + "/";
        ClearMapUI();
        UpdateMapUI(spritePath);
        scrollOneEffect.InitScrollView();
        UpdateLevelUI();
    }

    public override void UpdatePanel()
    {
        UpdateLevelUI();
    }

    public override void ExitPanel()
    {
        gameObject.SetActive(false);
    }
    #endregion

    // 加载资源的方法(预先加载该面板用到的所有资源一次,之后用到时则直接从工厂中取得)
    private void LoadResources()
    {
        mUIFacade.GetSprite(filePath + "AllClear");
        mUIFacade.GetSprite(filePath + "Carrot_1");
        mUIFacade.GetSprite(filePath + "Carrot_2");
        mUIFacade.GetSprite(filePath + "Carrot_3");

        for (int i = 1; i < 4; i++)
        {
            string spritePath = filePath + i + "/";
            mUIFacade.GetSprite(spritePath + "BG_Left");
            mUIFacade.GetSprite(spritePath + "BG_Right");
            for (int j = 1; j < 6; j++)
            {
                mUIFacade.GetSprite(spritePath + "Level_" + j);
            }
        }
        for (int i = 1; i < 13; i++)
        {
            mUIFacade.GetSprite(filePath + "Tower/Tower_" + i);
        }
    }

    // 更新地图UI(动态UI加载)
    private void UpdateMapUI(string spritePath)
    {
        leftIconImg.sprite = mUIFacade.GetSprite(spritePath + "BG_Left");
        rightIconImg.sprite = mUIFacade.GetSprite(spritePath + "BG_Right");
        for (int i = 0; i < 5; i++) // 更新五个关卡的卡片显示
        {
            levelContentImgList.Add(mUIFacade.CreateUI("Img_Level", levelContent));
            levelContentImgList[i].GetComponent<Image>().sprite = mUIFacade.GetSprite(spritePath + "Level_" + (i + 1).ToString()); // 更换关卡图片
            Stage stage = mUIFacade.GetLevelStage((currentBigLevelID - 1) * 5 + i);
            levelContentImgList[i].transform.Find("Img_AllClear").gameObject.SetActive(false);
            levelContentImgList[i].transform.Find("Img_Carrot").gameObject.SetActive(false);
            levelContentImgList[i].transform.Find("Img_RewardBG").gameObject.SetActive(false);
            levelContentImgList[i].transform.Find("Img_Lock").gameObject.SetActive(false);

            if (stage.unLocked) // 解锁的关卡
            {
                if (stage.allClear)
                {
                    levelContentImgList[i].transform.Find("Img_AllClear").gameObject.SetActive(true);
                }
                if (stage.carrotState != 0)
                {
                    Image carrotImg = levelContentImgList[i].transform.Find("Img_Carrot").GetComponent<Image>();
                    carrotImg.sprite = mUIFacade.GetSprite(filePath + "Carrot_" + stage.carrotState);
                    carrotImg.gameObject.SetActive(true);
                }
            }
            else // 未解锁关卡
            {
                if (stage.isRewardLevel) // 是奖励关卡
                {
                    levelContentImgList[i].transform.Find("Img_RewardBG").gameObject.SetActive(true);
                    levelContentImgList[i].transform.Find("Img_Lock").gameObject.SetActive(false);
                    Image monster = levelContentImgList[i].transform.Find("Img_RewardBG/Img_Monster").GetComponent<Image>();
                    monster.sprite = mUIFacade.GetSprite("MonsterNest/Monster/Baby/" + currentBigLevelID);
                    monster.SetNativeSize();
                    monster.transform.localScale = Vector3.one * 2;
                }
                else // 不是奖励关卡
                {
                    levelContentImgList[i].transform.Find("Img_RewardBG").gameObject.SetActive(false);
                    levelContentImgList[i].transform.Find("Img_Lock").gameObject.SetActive(true);
                }
            }
        }

        // 设置滚动视图的长度
        scrollOneEffect.SetContentLength(5);
    }

    // 清除地图UI的 方法(动态UI回收)
    private void ClearMapUI()
    {
        if (levelContentImgList.Count > 0)
        {
            for (int i = 0; i < levelContentImgList.Count; i++)
            {
                mUIFacade.PushItem(FactoryType.UI, "Img_Level", levelContentImgList[i]);
            }
            scrollOneEffect.InitContentLength();
            levelContentImgList.Clear();
        }
    }

    // 更新静态UI
    private void UpdateLevelUI()
    {
        if (towerContentImgList.Count > 0)
        {
            for (int i = 0; i < towerContentImgList.Count; i++)
            {
                towerContentImgList[i].GetComponent<Image>().sprite = null;
                mUIFacade.PushItem(FactoryType.UI, "Img_Tower", towerContentImgList[i]);
            }
            towerContentImgList.Clear();
        }

        Stage stage = mUIFacade.GetLevelStage((currentBigLevelID - 1) * 5 + (currentLevelID - 1));
        if (stage.unLocked) // 已经解锁的关卡
        {
            lockedButtonImg.SetActive(false); // 取消开始游戏的遮罩
        }
        else
        {
            lockedButtonImg.SetActive(true);
        }
        waveText.text = stage.totalWave.ToString(); // 波次数显示
        for (int i = 0; i < stage.towerListLen; i++)
        {
            towerContentImgList.Add(mUIFacade.CreateUI("Img_Tower", towerListEmp));
            towerContentImgList[i].GetComponent<Image>().sprite = mUIFacade.GetSprite(filePath + "Tower/Tower_" + stage.towerIDList[i]);
        }
    }

    // 进入游戏按钮的点击事件
    public void OnStartButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.SetCurrentStage((currentBigLevelID - 1) * 5 + (currentLevelID - 1));
        mUIFacade.currentScenePanelDict[StringManager.P_GameLoadPanel].EnterPanel();
        mUIFacade.ChangeSceneState(new NormalModelSceneState(mUIFacade));
    }

    /// <summary>
    /// 翻页执行的事件 toRight值为1时表示向右翻页,-1表示向左翻页
    /// </summary>
    public void ToNextLevel(int toRight)
    {
        currentLevelID += toRight;
        UpdatePanel();
    }
}
