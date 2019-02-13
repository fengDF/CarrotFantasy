using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinMove : MonoBehaviour
{
    private Text coinText;
    private Image coinImage;
    private Sprite[] coinSprites = new Sprite[2]; // 0:少量钱 1:大量钱
    [HideInInspector] public int prize;

    private void Awake()
    {
        coinText = transform.Find("Txt_Coin").GetComponent<Text>();
        coinImage = GetComponent<Image>();

        coinSprites[0] = GameController.Instance.GetSprite("NormalMordel/Game/Coin");
        coinSprites[1] = GameController.Instance.GetSprite("NormalMordel/Game/ManyCoin");
    }

    public void ShowCoin()
    {
        if(prize >= 10000) // Sprite不适配,暂不实现该需求
        {
            coinImage.sprite = coinSprites[1];
        }
        else
        {
            coinImage.sprite = coinSprites[0];
        }
        coinText.text = prize.ToString();

        transform.DOLocalMoveY(60, 1f);
        DOTween.To(() => coinImage.color, toColor => coinImage.color = toColor, new Color(1, 1, 1, 0), 1f);
        Tween tween = DOTween.To(() => coinText.color, toColor => coinText.color = toColor, new Color(1, 1, 1, 0), 1f);
        tween.OnComplete(DestroyCoin);
    }

    private void DestroyCoin()
    {
        transform.localPosition = Vector3.zero;
        coinImage.color = new Color(1, 1, 1, 1);
        coinText.color = new Color(1, 1, 1, 1);
        GameController.Instance.PushItem("CoinCanvas", transform.parent.gameObject);
    }
}
