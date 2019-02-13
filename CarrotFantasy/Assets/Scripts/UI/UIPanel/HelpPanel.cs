using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 帮助面板
/// </summary>
public class HelpPanel : BasePanel
{
    private GameObject helpPage;
    private GameObject monsterPage;
    private GameObject towerPage;
    private ScrollOneEffect helpScrollEffect;
    private ScrollOneEffect towerScrollEffect;
    private Tween enterTween;

    protected override void Awake()
    {
        base.Awake();

        // 赋值引用
        helpPage = transform.Find("HelpPage").gameObject;
        monsterPage = transform.Find("MonsterPage").gameObject;
        towerPage = transform.Find("TowerPage").gameObject;
        helpScrollEffect = helpPage.transform.Find("Scroll View").GetComponent<ScrollOneEffect>();
        towerScrollEffect = towerPage.transform.Find("Scroll View").GetComponent<ScrollOneEffect>();

        // 存储进入面板的动画
        enterTween = transform.DOLocalMoveX(0, 0.5f);
        enterTween.SetAutoKill(false);
        enterTween.Pause();
    }

    public override void InitPanel()
    {
        transform.SetSiblingIndex(5); // 设置渲染层级

        // 初始化ScorllView的位置
        helpScrollEffect.InitScrollView();
        towerScrollEffect.InitScrollView();

        OnHelpButtonClick(); // 设置初始页面

        // 其他场景状态的处理
        if (transform.localPosition == Vector3.zero)
        {
            gameObject.SetActive(false); // 失活面板
            enterTween.PlayBackwards();
        }

        transform.localPosition = new Vector3(1920, 0, 0); // 初始化位置
    }


    public override void EnterPanel()
    {
        gameObject.SetActive(true); // 激活面板

        // 初始化ScorllView的位置
        helpScrollEffect.InitScrollView();
        towerScrollEffect.InitScrollView();


        OnHelpButtonClick();
        enterTween.PlayForward();
    }

    public override void ExitPanel()
    {
        // 如果是在冒险模式选择场景下
        if (mUIFacade.currentSceneState.GetType() == typeof(NormalOptionSceneState))
        {
            mUIFacade.ChangeSceneState(new MainSceneState(mUIFacade));
        }
        else
        {
            enterTween.PlayBackwards();
            mUIFacade.currentScenePanelDict[StringManager.P_MainPanel].EnterPanel();
        }
    }

    #region 按钮监听事件

    public void OnHelpButtonClick()
    {
        if (!helpPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        helpPage.SetActive(true);
        monsterPage.SetActive(false);
        towerPage.SetActive(false);
    }

    public void OnMonsterButtonClick()
    {
        if (!monsterPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        helpPage.SetActive(false);
        monsterPage.SetActive(true);
        towerPage.SetActive(false);
    }

    public void OnTowerButtonClick()
    {
        if (!towerPage.activeSelf) mUIFacade.PlayButtonAudioEffect();
        helpPage.SetActive(false);
        monsterPage.SetActive(false);
        towerPage.SetActive(true);
    }

    public void OnHomeButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        ExitPanel();
    }

    #endregion
}
