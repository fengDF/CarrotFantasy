using UnityEngine.SceneManagement;

/// <summary>
/// 冒险模式选择场景状态
/// </summary>
public class NormalOptionSceneState : BaseSceneState
{
    public NormalOptionSceneState(UIFacade uIFacade) : base(uIFacade) { }

    public override void EnterScene()
    {
        mUIFacade.AddPanelToDict(StringManager.P_GameNormalOptionPanel);
        mUIFacade.AddPanelToDict(StringManager.P_GameNormalBigLevelPanel);
        mUIFacade.AddPanelToDict(StringManager.P_GameNormalLevelPanel);
        mUIFacade.AddPanelToDict(StringManager.P_GameLoadPanel);
        mUIFacade.AddPanelToDict(StringManager.P_HelpPanel);

        base.EnterScene();
        mUIFacade.currentScenePanelDict[StringManager.P_GameNormalBigLevelPanel].EnterPanel();

    }

    public override void ExitScene()
    {
        base.ExitScene();

        // 通过反射得到下一个场景状态的类类型
        if (mUIFacade.currentSceneState.GetType() == typeof(NormalModelSceneState))
        {
            SceneManager.LoadScene(4);
        }
        else if (mUIFacade.currentSceneState.GetType() == typeof(MainSceneState))
        {
            SceneManager.LoadScene(1);

        }
    }
}
