using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 设置面板
/// </summary>
public class SetPanel : BasePanel
{
    // 其他分页面的引用
    private GameObject optionPage;
    private GameObject dataPage;
    private GameObject producerPage;
    private GameObject resetPage;

    private Tween enterTween; // 进入Panel的动画,存储起来以便倒播

    // 控制音乐音效的开关
    private bool playBGMusic = true;
    private bool playEffectMusic = true;
    public Sprite[] audioSprites; // 音频开关显示的图片 (0: 音效开 1:音效关 2:音乐开 3:音乐关)
    private Image effectAudioImage; // 音效图片引用
    private Image bgmImage; // 背景音乐图片引用

    // 数据分页显示的数据文本
    public Text[] dataTexts;

    protected override void Awake()
    {
        base.Awake();

        // 进入面板动画设置
        enterTween = transform.DOLocalMoveX(0, 0.5f);
        enterTween.SetAutoKill(false);
        enterTween.Pause();

        // 赋值分页面的引用
        optionPage = transform.Find("OptionPage").gameObject;
        dataPage = transform.Find("DataPage").gameObject;
        producerPage = transform.Find("ProducerPage").gameObject;
        resetPage = optionPage.transform.Find("ResetPage").gameObject;

        // 音频Image组件引用
        effectAudioImage = optionPage.transform.Find("Btn_AudioEffect").GetComponent<Image>();
        bgmImage = optionPage.transform.Find("Btn_AudioBGM").GetComponent<Image>();
    }

    public override void InitPanel()
    {
        transform.localPosition = new Vector3(-1920, 0, 0);
        transform.SetSiblingIndex(2);
    }

    public override void EnterPanel()
    {
        OnOptionButtonClick();
        enterTween.PlayForward();
    }

    public override void ExitPanel()
    {
        enterTween.PlayBackwards();
        mUIFacade.currentScenePanelDict[StringManager.P_MainPanel].EnterPanel();
    }

    // 数据显示的方法
    private void ShowData()
    {
        int[] data = mUIFacade.GetStatisticsData();
        for(int i = 0;i < data.Length; i++)
        {
            dataTexts[i].text = data[i].ToString();
        }
    }

    // 重置游戏的方法
    public void ResetGame()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.ResetGame();
        resetPage.SetActive(false);
    }

    #region 按钮监听事件

    public void OnOptionButtonClick()
    {
        if(!optionPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        // 显示选项页面
        optionPage.SetActive(true);
        dataPage.SetActive(false);
        producerPage.SetActive(false);
    }

    public void OnDataButtonClick()
    {
        if (!dataPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        // 显示数据页面
        ShowData(); // 更新数据页面的统计数据 
        optionPage.SetActive(false);
        dataPage.SetActive(true);
        producerPage.SetActive(false);
    }

    public void OnProducerButtonClick()
    {
        if (!producerPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        // 显示制作方页面
        optionPage.SetActive(false);
        dataPage.SetActive(false);
        producerPage.SetActive(true);
    }

    // 处理音乐开启关闭的方法
    public void OnBGMButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        playBGMusic = !playBGMusic;

        if (playBGMusic)
        {
            bgmImage.sprite = audioSprites[2];
        }
        else
        {
            bgmImage.sprite = audioSprites[3];
        }

        mUIFacade.BGMMusicButtonClick();
    }

    // 处理音效开启关闭的方法
    public void OnEffectMusicButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        playEffectMusic = !playEffectMusic;

        if (playEffectMusic)
        {
            effectAudioImage.sprite = audioSprites[0];
        }
        else
        {
            effectAudioImage.sprite = audioSprites[1];
        }

        mUIFacade.EffectMusicButtonClick();
    }

    public void OnResetButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        resetPage.SetActive(true);
    }

    public void OnCloseResetButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        resetPage.SetActive(false);
    }

    public void OnExitButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        ExitPanel();
    }
    #endregion
}
