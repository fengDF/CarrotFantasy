using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单场景状态
/// </summary>
public class MainSceneState : BaseSceneState
{
    public MainSceneState(UIFacade uIFacade) : base(uIFacade) { }

    public override void EnterScene()
    {
        mUIFacade.AddPanelToDict(StringManager.P_MainPanel);
        mUIFacade.AddPanelToDict(StringManager.P_HelpPanel);
        mUIFacade.AddPanelToDict(StringManager.P_SetPanel);
        mUIFacade.AddPanelToDict(StringManager.P_GameLoadPanel);

        base.EnterScene();
        GameManager.Instance.AudioManager.PlayBGMusic(GameManager.Instance.GetAudioClip("Main/BGMusic"),0.2f);
    }

    public override void ExitScene()
    {
        base.ExitScene();

        // 通过反射得到下一个场景状态的类类型
        if (mUIFacade.currentSceneState.GetType() == typeof(NormalOptionSceneState))
        {
            SceneManager.LoadScene(2);
        }
        else if (mUIFacade.currentSceneState.GetType() == typeof(BossOptionSceneState))
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            SceneManager.LoadScene(6);
        }
    }
}
