using UnityEngine.SceneManagement;

/// <summary>
/// 怪物窝场景状态
/// </summary>
public class MonsterNestSceneState : BaseSceneState
{
    public MonsterNestSceneState(UIFacade uIFacade) : base(uIFacade) { }

    public override void EnterScene()
    {
        mUIFacade.AddPanelToDict(StringManager.P_MonsterNestPanel);
        mUIFacade.AddPanelToDict(StringManager.P_GameLoadPanel);
        GameManager.Instance.AudioManager.PlayBGMusic(GameManager.Instance.GetAudioClip("MonsterNest/BGMusic02"), 1f);

        base.EnterScene();
    }

    public override void ExitScene()
    {
        base.ExitScene();

        if (mUIFacade.currentSceneState.GetType() == typeof(MainSceneState))
        {
            SceneManager.LoadScene(1);
        }
    }
}
