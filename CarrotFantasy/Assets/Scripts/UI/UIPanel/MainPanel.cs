using UnityEngine;
using DG.Tweening;

/// <summary>
/// 主菜单面板
/// </summary>
public class MainPanel : BasePanel
{
    [HideInInspector] public Tween[] mainTweens; // 存储左右移动画(0:右移 1:左移)

    // 引用
    private Animator carrotAnimator;
    private Transform monsterTrans;
    private Transform cloudTrans;

    private Tween exitTween; // 离开页面移动的动画(存储动画以便回到页面时进行倒播)

    protected override void Awake()
    {
        base.Awake();

        transform.SetSiblingIndex(8); // 设置渲染层级
        // 赋值成员变量的引用
        carrotAnimator = transform.Find("Emp_Carrot").GetComponent<Animator>();
        monsterTrans = transform.Find("Img_Monster");
        cloudTrans = transform.Find("Img_Cloud");

        carrotAnimator.Play("CarrotGrow"); // 控制播放萝卜生长动画

        // 存储Panel左右移动画
        mainTweens = new Tween[2];
        mainTweens[0] = transform.DOLocalMoveX(1920, 0.5f);
        mainTweens[0].SetAutoKill(false);
        mainTweens[0].Pause();
        mainTweens[1] = transform.DOLocalMoveX(-1920, 0.5f);
        mainTweens[1].SetAutoKill(false);
        mainTweens[1].Pause();

        PlayUIAnim();
    }

    public override void EnterPanel()
    {
        carrotAnimator.Play("CarrotGrow");
        if (exitTween != null) // 如果是通过动画从该面板转换到其他面板,则返回该面板进行动画倒播
        {
            exitTween.PlayBackwards();
        }
        cloudTrans.gameObject.SetActive(true);
    }

    public override void ExitPanel()
    {
        exitTween.PlayForward();
        cloudTrans.gameObject.SetActive(false);
    }

    // UI动画
    private void PlayUIAnim()
    {
        monsterTrans.DOLocalMoveY(150f, 2f).SetLoops(-1, LoopType.Yoyo);
        cloudTrans.DOLocalMoveX(1200f, 8f).SetLoops(-1, LoopType.Restart);
    }

    #region 按钮监听事件

    public void OnNormalButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.currentScenePanelDict[StringManager.P_GameLoadPanel].EnterPanel();
        mUIFacade.ChangeSceneState(new NormalOptionSceneState(mUIFacade));
    }

    // TODO
    public void OnBossButtonClick()
    {

    }

    public void OnMonsterButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.currentScenePanelDict[StringManager.P_GameLoadPanel].EnterPanel();
        mUIFacade.ChangeSceneState(new MonsterNestSceneState(mUIFacade));
    }

    public void OnSetButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        // 面板向右动画(显示Set面板)
        exitTween = mainTweens[0];
        ExitPanel();
        mUIFacade.currentScenePanelDict[StringManager.P_SetPanel].EnterPanel();
    }


    public void OnHelpButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        // 面板向左动画(显示Help面板)
        exitTween = mainTweens[1];
        ExitPanel();
        mUIFacade.currentScenePanelDict[StringManager.P_HelpPanel].EnterPanel();
    }

    public void OnExitButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.SaveData();
        Application.Quit();
    }

    #endregion
}
