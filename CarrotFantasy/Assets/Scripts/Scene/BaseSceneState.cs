
/// <summary>
/// 场景状态的基类
/// </summary>
public class BaseSceneState : IBaseSceneState
{
    protected UIFacade mUIFacade;

    public BaseSceneState(UIFacade uIFacade)
    {
        mUIFacade = uIFacade;
    }

    public virtual void EnterScene()
    {
        mUIFacade.InitUIPanelDict();
    }

    public virtual void ExitScene()
    {
        mUIFacade.ClearUIPanelDict();
    }
}
