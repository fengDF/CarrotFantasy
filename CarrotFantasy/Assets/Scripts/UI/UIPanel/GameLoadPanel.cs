
/// <summary>
/// 游戏中加载面板
/// </summary>
public class GameLoadPanel : BasePanel
{
    public override void InitPanel()
    {
        gameObject.SetActive(false);
        transform.SetSiblingIndex(8);
    }

    public override void EnterPanel()
    {
        gameObject.SetActive(true);
    }
}
