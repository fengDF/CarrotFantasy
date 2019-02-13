using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 冒险模式大关卡选择面板
/// </summary>
public class GameNormalBigLevelPanel : BasePanel
{
    private Transform bigLevelContent;
    private int bigPageCount;
    private ScrollOneEffect bigLevelScroll;
    private Transform[] bigLevelItems;
    private bool hasRigisterEvent;

    protected override void Awake()
    {
        base.Awake();

        bigLevelContent = transform.Find("Scroll View").GetComponent<ScrollRect>().content;
        bigPageCount = bigLevelContent.childCount;
        bigLevelScroll = transform.Find("Scroll View").GetComponent<ScrollOneEffect>();
        bigLevelItems = new Transform[bigPageCount];

        // 初始化循环更新每个大关卡的数据信息
        for (int i = 0; i < bigPageCount; i++)
        {
            bigLevelItems[i] = bigLevelContent.GetChild(i);
            BigLevelData data = mUIFacade.GetBigLevelData(i);
            ShowBigLevelUI(data.unLocked, data.lockedLevelNum, data.totalLevelNum, bigLevelItems[i], i + 1);
        }
        hasRigisterEvent = true; // 已经为大关卡按钮注册事件 不重复注册
    }

    // 每次进入该Panel时更新数据显示
    private void OnEnable()
    {
        // 循环更新每个大关卡的数据信息
        for (int i = 0; i < bigPageCount; i++)
        {
            BigLevelData data = mUIFacade.GetBigLevelData(i);
            ShowBigLevelUI(data.unLocked, data.lockedLevelNum, data.totalLevelNum, bigLevelItems[i], i + 1);
        }
    }

    public override void EnterPanel()
    {
        gameObject.SetActive(true);
        bigLevelScroll.InitScrollView();
    }

    public override void ExitPanel()
    {
        gameObject.SetActive(false);
    }

    // 更新显示大关卡数据UI
    private void ShowBigLevelUI(bool unLocked, int lockedLevelNum, int totalLevelNum, Transform bigLevel, int bigLevelID)
    {
        Button btnBigLevel = bigLevel.GetComponent<Button>();
        if (!unLocked) // 未解锁状态
        {
            bigLevel.Find("Img_Lock").gameObject.SetActive(true);
            bigLevel.Find("Img_PageCount").gameObject.SetActive(false);
            btnBigLevel.interactable = false;
        }
        else // 解锁状态
        {
            bigLevel.Find("Img_Lock").gameObject.SetActive(false);
            bigLevel.Find("Img_PageCount").gameObject.SetActive(true);
            bigLevel.Find("Img_PageCount/Txt_PageCount").GetComponent<Text>().text = lockedLevelNum + "/" + totalLevelNum;
            btnBigLevel.interactable = true;
        }
        if (hasRigisterEvent) return; // 如果已经注册过按钮监听事件,则不重复注册
        btnBigLevel.onClick.AddListener(() =>
        {
            mUIFacade.PlayButtonAudioEffect();
            mUIFacade.currentScenePanelDict[StringManager.P_GameNormalBigLevelPanel].ExitPanel(); // 离开大关卡选择页面
                GameNormalLevelPanel gameNormalLevelPanel = mUIFacade.currentScenePanelDict[StringManager.P_GameNormalLevelPanel] as GameNormalLevelPanel;
            gameNormalLevelPanel.EnterPanelFromBigLevel(bigLevelID); // 进入小关卡选择页面
                GameNormalOptionPanel gameNormalOptionPanel = mUIFacade.currentScenePanelDict[StringManager.P_GameNormalOptionPanel] as GameNormalOptionPanel;
            gameNormalOptionPanel.isInBigLevel = false;
        });
    }

    #region 按钮监听事件

    public void OnLeftButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        bigLevelScroll.ToNextPage(-1);
    }

    public void OnRightButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        bigLevelScroll.ToNextPage(1);
    }

    #endregion
}
