
/// <summary>
/// 冒险模式选择面板
/// </summary>
public class GameNormalOptionPanel : BasePanel
{
    public bool isInBigLevel = true;

    public override void InitPanel()
    {
        isInBigLevel = true;
    }

    #region 按钮监听事件

    public void OnReturnButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        if (isInBigLevel) // 如果是在大关卡面板点击则返回主界面
        {
            mUIFacade.ChangeSceneState(new MainSceneState(mUIFacade));
        }
        else
        {
            mUIFacade.currentScenePanelDict[StringManager.P_GameNormalLevelPanel].ExitPanel();
            mUIFacade.currentScenePanelDict[StringManager.P_GameNormalBigLevelPanel].EnterPanel();
        }
        isInBigLevel = true;
    }

    public void OnHelpButtonClick()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.currentScenePanelDict[StringManager.P_HelpPanel].EnterPanel();
    }

    #endregion
}
