using UnityEngine.SceneManagement;


/// <summary>
/// 开始加载游戏场景状态
/// </summary>
public class StartLoadSceneState : BaseSceneState
{
    public StartLoadSceneState(UIFacade uIFacade) : base(uIFacade) { }

    public override void EnterScene()
    {
        mUIFacade.AddPanelToDict(StringManager.P_StartLoadPanel);

        base.EnterScene();
    }

    public override void ExitScene()
    {
        base.ExitScene();
        SceneManager.LoadScene(1);
    }
}
