using UnityEngine;

/// <summary>
/// Panel基类
/// </summary>
public class BasePanel : MonoBehaviour, IBasePanel
{
    protected UIFacade mUIFacade;

    protected virtual void Awake()
    {
        mUIFacade = GameManager.Instance.UIManager.mUIFacade;
    }

    public virtual void InitPanel()
    {
        
    }

    public virtual void EnterPanel()
    {
        
    }

    public virtual void UpdatePanel()
    {
        
    }

    public virtual void ExitPanel()
    {
        
    }
}
