using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 所有的道具类
/// </summary>
public class Item : MonoBehaviour
{
    [HideInInspector] public GridPoint gridPoint;
    [HideInInspector] public int itemID; // 手动赋值
    private GameController gameController;
    private Slider hpSlider;

    private int price; // 销毁得到的金币
    private int hp;
    private int currentHP;
    private float hideTimer; // 一定时间不攻击自动隐藏血条的计时器
    private bool showHP; // 显示血条的开关

    private void Awake()
    {
        hpSlider = transform.Find("ItemCanvas/HpSlider").GetComponent<Slider>();
        InitItem();
    }

    private void OnEnable()
    {
        gameController = GameController.Instance;
        InitItem();
    }

    private void Update()
    {
        if (showHP)
        {
            if (hideTimer <= 0)
            {
                hpSlider.gameObject.SetActive(false);
                showHP = false;
                hideTimer = 3;
            }
            else
            {
                hideTimer -= Time.deltaTime;
                hpSlider.gameObject.SetActive(true);

            }
        }
    }

    private void InitItem()
    {
        price = 500 - 50 * itemID;
        hp = 1000 - 100 * itemID;
        currentHP = hp;
        hideTimer = 3f;
        showHP = false;
        hpSlider.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;// 如果点击到的是UI

        if (gameController.targetTrans == null)
        {
            gameController.targetTrans = transform;
            gameController.ShowSignal();
        }
        else if (gameController.targetTrans != transform) // 转换新目标
        {
            gameController.HideSignal();
            gameController.targetTrans = transform;
            gameController.ShowSignal();
        }
        else // 两次点击的是同一个目标
        {
            gameController.HideSignal();
        }
    }

    // 承受伤害的方法
    private void TakeDamage(int value)
    {
        showHP = true;
        hideTimer = 3;
        currentHP -= value;
        if (currentHP <= 0)
        {
            // 销毁道具
            DestroyItem();
            return;
        }
        hpSlider.value = (float)currentHP / hp;
    }

    private void DestroyItem()
    {
        if (gameController.targetTrans == transform)
        {
            gameController.HideSignal();
        }
        gameController.PlayAudioEffect("NormalMordel/Item");
        // 生成金币奖励
        GameObject coin = gameController.GetItem("CoinCanvas");
        coin.transform.Find("Img_Coin").GetComponent<CoinMove>().prize = price;
        coin.transform.Find("Img_Coin").GetComponent<CoinMove>().ShowCoin();
        coin.transform.SetParent(gameController.transform);
        coin.transform.position = transform.position;
        // 增加玩家金币
        gameController.ChangeCoinNum(price);
        gameController.clearItemNum++;
        // 生成销毁特效
        GameObject effect = gameController.GetItem("DestroyEffect");
        effect.transform.SetParent(gameController.transform);
        effect.transform.position = transform.position;
        // 将销毁的道具放回对象池
        gameController.PushItem(gameController.currentStage.bigLevelID + "/Item/" + itemID, gameObject);
        InitItem();
        gridPoint.InitGridState();
    }
}
