using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 冒险模式面板
/// </summary>
public class NormalModelPanel : BasePanel
{
    // 控制的页面
    private GameObject topPageGO;
    private GameObject gameOverPageGO;
    private GameObject winPageGO;
    private GameObject menuPageGO;
    private GameObject finalWaveImg;
    private GameObject startCountDown;
    private GameObject prizePageGO;

    // 引用
    public GameController gameController;
    [HideInInspector] public TopPage topPage;

    [HideInInspector] public int totalRound;

    protected override void Awake()
    {
        base.Awake();

        transform.SetSiblingIndex(1);

        topPageGO = transform.Find("TopPage").gameObject;
        gameOverPageGO = transform.Find("GameOverPage").gameObject;
        winPageGO = transform.Find("GameWinPage").gameObject;
        menuPageGO = transform.Find("MenuPage").gameObject;
        prizePageGO = transform.Find("PrizePage").gameObject;
        startCountDown = transform.Find("StartUI").gameObject;
        finalWaveImg = transform.Find("Img_FinalWave").gameObject;

        topPage = topPageGO.GetComponent<TopPage>();
    }

    public override void EnterPanel()
    {
        gameObject.SetActive(true);
        gameController = GameController.Instance;
        totalRound = gameController.currentStage.totalWave;
        topPageGO.SetActive(true);
        InvokeRepeating("PlayAudio", 0, 1);
        startCountDown.SetActive(true);
        Invoke("StartGame", 3);
    }

    public override void UpdatePanel()
    {
        topPage.UpdateCoinText();
        topPage.UpdateRoundText();
    }

    // 播放开始游戏倒计时的方法
    private void PlayAudio()
    {
        gameController.PlayAudioEffect("NormalMordel/CountDown");
    }

    // 开始游戏的处理
    private void StartGame()
    {
        gameController.PlayAudioEffect("NormalMordel/GO");
        gameController.StartGame();
        startCountDown.SetActive(false);
        CancelInvoke();
    }

    // 更新波次Text显示的方法
    public void ShowRound(Text text)
    {
        int currentRound = gameController.level.currentRound + 1;
        if (currentRound < 10)
        {
            text.text = "0  " + currentRound;
        }
        else
        {
            text.text = currentRound / 10 + "  " + currentRound % 10;
        }
    }

    #region Panel作为外观者
    // 下层向UIFacade发送数据
    public void SetMonsterData(int cookies, int milks, int nest)
    {
        mUIFacade.SetMonsterData(cookies, milks, nest);
    }

    // 下层向Facade发送宠物蛋数据
    public void SetMonsterEgg(MonsterPetData monsterPetData)
    {
        mUIFacade.SetMonsterEgg(monsterPetData);
    }

    // 下层向UIFacade获取数据
    public List<MonsterPetData> GetMonsterPetData()
    {
        return mUIFacade.GetMonsterPetData();
    }
    #endregion

    #region 与关卡处理有关的方法

    // 更新玩家基础数据
    private void UpdatePlayManagerData()
    {
        int getCoin = 0;
        if (gameController.Coin > 500) getCoin = gameController.Coin - 500;
        mUIFacade.SetStatisticData(getCoin, gameController.clearItemNum, gameController.killMonsterTotalNum);
    }

    // 重玩游戏的方法
    public void RePlay()
    {
        UpdatePlayManagerData();
        mUIFacade.ChangeSceneState(new NormalModelSceneState(mUIFacade));
        Invoke("ResetGame", 2f);
    }

    // 重置当前关卡游戏
    private void ResetGame()
    {
        SceneManager.LoadScene(4);
        ResetUI();
    }

    // 重置当前关卡UI
    private void ResetUI()
    {
        topPageGO.SetActive(false);
        gameOverPageGO.SetActive(false);
        winPageGO.SetActive(false);
        menuPageGO.SetActive(false);
        gameObject.SetActive(false);
    }

    // 选择其他关卡的方法
    public void ChooseOtherLevel()
    {
        UpdatePlayManagerData();
        mUIFacade.ChangeSceneState(new NormalOptionSceneState(mUIFacade));
        Invoke("ToOtherScene", 2f);
    }

    public void ToOtherScene()
    {
        ResetUI();
        SceneManager.LoadScene(2);
    }

    #endregion

    #region 一些页面显示隐藏的方法

    // 最后一波怪显示的方法
    public void ShowFinalWave()
    {
        gameController.PlayAudioEffect("NormalMordel/Finalwave");
        finalWaveImg.SetActive(true);
        Invoke("CloseFinalWave", 0.5f);
    }

    private void CloseFinalWave()
    {
        finalWaveImg.SetActive(false);
    }

    // 显示菜单按钮的方法
    public void ShowMenu()
    {
        menuPageGO.SetActive(true);
        gameController.isPause = true;
    }

    // 隐藏菜单按钮的方法
    public void HideMenu()
    {
        gameController.isPause = false;
        menuPageGO.SetActive(false);
    }

    // 显示宠物页面的方法
    public void ShowPrize()
    {
        prizePageGO.SetActive(true);
    }

    // 隐藏宠物页面的方法
    public void HidePrize()
    {
        prizePageGO.SetActive(false);
        if (!topPage.isPause) gameController.isPause = false;
    }

    // 显示失败面板
    public void ShowGameOver()
    {
        UpdatePlayManagerData();
        gameOverPageGO.SetActive(true);
        gameController.PlayAudioEffect("NormalMordel/Lose");
    }

    // 显示胜利面板
    public void ShowGameWin()
    {
        int index = gameController.currentStage.levelID - 1 + (gameController.currentStage.bigLevelID - 1) * 5;
        Stage stage = mUIFacade.GetLevelStage(index);
        // 关卡道具徽章的更新
        if (gameController.IsAllClearItem())
        {
            stage.allClear = true;
        }
        // 关卡萝卜状态的更新
        int carrotState = gameController.GetCarrotHealth();
        if (stage.carrotState == 0 || (stage.carrotState != 0 && carrotState < stage.carrotState))
        {
            stage.carrotState = carrotState;
        }
        // 解锁下一个关卡(隐藏关卡(4,9,14))
        if (index < 14 && (index + 2) % 5 != 0) // 关卡长度以及每个小关卡的隐藏关卡都应从PlayerManager中取得
        {
            mUIFacade.SetLevelData(index + 1);
        }
        // 一些统计数据的更新
        UpdatePlayManagerData();
        mUIFacade.SetNormalNum();

        winPageGO.SetActive(true);
        gameController.PlayAudioEffect("NormalMordel/Perfect");
    }

    #endregion

    public void PlayButtonAudioEffect()
    {
        mUIFacade.PlayButtonAudioEffect();
    }
}
