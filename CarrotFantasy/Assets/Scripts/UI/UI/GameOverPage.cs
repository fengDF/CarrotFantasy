using UnityEngine;
using UnityEngine.UI;

public class GameOverPage : MonoBehaviour
{
    private NormalModelPanel normalModelPanel;
    private Text currentRoundText;
    private Text totalRoundText;
    private Text currentLevelText;

    private void Awake()
    {
        normalModelPanel = GetComponentInParent<NormalModelPanel>();
        currentRoundText = transform.Find("Img_BG/Txt_RoundCount").GetComponent<Text>();
        totalRoundText = transform.Find("Img_BG/Txt_TotalCount").GetComponent<Text>();
        currentLevelText = transform.Find("Img_BG/Txt_CurrentLevel").GetComponent<Text>();
    }

    private void OnEnable()
    {
        totalRoundText.text = normalModelPanel.totalRound.ToString();
        currentLevelText.text = (normalModelPanel.gameController.currentStage.levelID + (normalModelPanel.gameController.currentStage.bigLevelID - 1) * 5).ToString();
        normalModelPanel.ShowRound(currentRoundText);
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
