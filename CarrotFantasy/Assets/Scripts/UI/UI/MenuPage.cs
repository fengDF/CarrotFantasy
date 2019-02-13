using UnityEngine;

public class MenuPage : MonoBehaviour
{
    private NormalModelPanel normalModelPanel;

    private void Awake()
    {
        normalModelPanel = transform.GetComponentInParent<NormalModelPanel>();
    }

    public void OnContinueButtonClick()
    {
        normalModelPanel.PlayButtonAudioEffect();
        normalModelPanel.HideMenu();
    }

    public void OnReplayButtonClick()
    {
        normalModelPanel.PlayButtonAudioEffect();
        normalModelPanel.RePlay();
    }

    public void OnChooseLevelButtonClick()
    {
        normalModelPanel.PlayButtonAudioEffect();
        normalModelPanel.ChooseOtherLevel();
    }
}
