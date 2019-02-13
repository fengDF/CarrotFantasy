using UnityEngine;
using UnityEngine.UI;

public class TopPage : MonoBehaviour
{
    // 引用
    private Text currentRoundText;
    private Text totalRoundText;
    private Text coinText;
    private Image gameSpeedImg;
    private Image pauseImg;
    private GameObject showInPauseText;
    private GameObject showInPlayingText;
    private NormalModelPanel normalModelPanel;

    // 图片资源
    private Sprite[] gameSpeedSprites = new Sprite[2]; // 0: 一倍速 1:二倍速
    private Sprite[] pauseSprites = new Sprite[2]; // 0:暂停按钮 1:继续按钮

    // 属性值
    private bool isNormalSpeed; // 是否是一倍速的开关
    public bool isPause;

    private void Awake()
    {
        normalModelPanel = GetComponentInParent<NormalModelPanel>();
        showInPlayingText = transform.Find("Emp_PlayingWaves").gameObject;
        showInPauseText = transform.Find("Emp_Pause").gameObject;
        currentRoundText = showInPlayingText.transform.Find("Img_NowWave/Txt_NowWave").GetComponent<Text>();
        totalRoundText = showInPlayingText.transform.Find("Txt_Instruction").GetComponent<Text>();
        coinText = transform.Find("Txt_Coin").GetComponent<Text>();
        gameSpeedImg = transform.Find("Btn_GameSpeed").GetComponent<Image>();
        pauseImg = transform.Find("Btn_Pause").GetComponent<Image>();

        gameSpeedSprites[0] = normalModelPanel.gameController.GetSprite("NormalMordel/OneSpeed");
        gameSpeedSprites[1] = normalModelPanel.gameController.GetSprite("NormalMordel/TwoSpeed");
        pauseSprites[0] = normalModelPanel.gameController.GetSprite("NormalMordel/Pause");
        pauseSprites[1] = normalModelPanel.gameController.GetSprite("NormalMordel/Replay");

        isNormalSpeed = true;
    }

    private void OnEnable()
    {
        UpdateCoinText();
        UpdateRoundText();
        totalRoundText.text = "/" + normalModelPanel.totalRound + "波怪物";
        gameSpeedImg.sprite = gameSpeedSprites[0];
        pauseImg.sprite = pauseSprites[0];
        isNormalSpeed = true;
        isPause = false;
        showInPlayingText.SetActive(true);
        showInPauseText.SetActive(false);
    }

    public void UpdateCoinText()
    {
        coinText.text = normalModelPanel.gameController.Coin.ToString();
    }

    public void UpdateRoundText()
    {
        normalModelPanel.ShowRound(currentRoundText);
    }

    public void ChangeSpeed()
    {
        normalModelPanel.PlayButtonAudioEffect();
        isNormalSpeed = !isNormalSpeed;
        if (isNormalSpeed)
        {
            normalModelPanel.gameController.gameSpeed = 1;
            gameSpeedImg.sprite = gameSpeedSprites[0];
        }
        else
        {
            normalModelPanel.gameController.gameSpeed = 2;
            gameSpeedImg.sprite = gameSpeedSprites[1];
        }
    }

    public void PauseGame()
    {
        normalModelPanel.PlayButtonAudioEffect();
        isPause = !isPause;
        if (isPause)
        {
            normalModelPanel.gameController.isPause = true;
            pauseImg.sprite = pauseSprites[1];
            showInPauseText.SetActive(true);
            showInPlayingText.SetActive(false);
        }
        else
        {
            normalModelPanel.gameController.isPause = false;
            pauseImg.sprite = pauseSprites[0];
            showInPauseText.SetActive(false);
            showInPlayingText.SetActive(true);
        }
    }

    public void ShowMenu()
    {
        normalModelPanel.PlayButtonAudioEffect();
        normalModelPanel.ShowMenu();
    }
}
