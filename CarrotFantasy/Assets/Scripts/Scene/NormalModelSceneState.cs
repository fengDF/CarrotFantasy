
/// <summary>
/// 冒险模式场景状态
/// </summary>
public class NormalModelSceneState : BaseSceneState
{
    public NormalModelSceneState(UIFacade uIFacade) : base(uIFacade) { }

    public override void EnterScene()
    {
        mUIFacade.AddPanelToDict(StringManager.P_GameLoadPanel);
        mUIFacade.AddPanelToDict(StringManager.P_NormalModelPanel);
        
        base.EnterScene();

        GameManager.Instance.AudioManager.CloseBGMusic();
    }

    public override void ExitScene()
    {
        base.ExitScene();

        GameManager.Instance.AudioManager.OpenBGMusic();
    }
}
