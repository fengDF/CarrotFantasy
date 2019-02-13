using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Monster : MonoBehaviour
{
    // 怪物的属性值
    public int monsterID;
    public int hp;
    public int currentHP;
    public float moveSpeed; //当前移动速度
    public float initMoveSpeed; // 初始移动速度
    public int prize; // 奖励金币

    // 相关引用
    private GameController gameController;
    private List<Vector3> monsterPointList; // 当前关卡的怪物路点
    private Animator animator;
    private Slider hpSlider; // 显示生命值的滑动条

    // 用于计数的成员变量和开关
    private int roadIndex = 1; // 寻路的索引
    private bool reachCarrot; // 到达终点的开关

    // 关于减速buff的成员变量
    public GameObject shit; // 沾满便便的游戏物体
    private bool hasDecreaseSpeed; // 是否减速的开关
    private float decreaseTime; // 减速的计时器
    private float decreaseTimer;

    // 资源
    private RuntimeAnimatorController runtimeAnimator; // 默认动画控制器

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hpSlider = transform.Find("MonsterCanvas/Slider_HP").GetComponent<Slider>();
        hpSlider.gameObject.SetActive(false);
        shit = transform.Find("TShit").gameObject;
    }

    private void OnEnable()
    {
        gameController = GameController.Instance;
        monsterPointList = gameController.mapMaker.monsterPointPosList;
        Turning();
        hpSlider.gameObject.transform.eulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if (gameController.isPause || gameController.gameOver) return;
        if (!reachCarrot) // 没有到达终点->寻路
        {
            transform.position = Vector3.Lerp(transform.position, monsterPointList[roadIndex],
                1f / Vector3.Distance(transform.position, monsterPointList[roadIndex]) * moveSpeed * Time.deltaTime * gameController.gameSpeed);
            if (Vector3.Distance(transform.position, monsterPointList[roadIndex]) <= 0.01f)
            {
                transform.position = monsterPointList[roadIndex];
                roadIndex++;
                hpSlider.gameObject.transform.eulerAngles = Vector3.zero;
                Turning();

                if (roadIndex == monsterPointList.Count)
                {
                    reachCarrot = true;
                }
            }
        }
        else // 到达终点,销毁自身
        {
            DestroyMonster();
            // TODO 萝卜减血
            gameController.ChangeCarrotHP();
        }

        if (hasDecreaseSpeed)
        {
            if (decreaseTimer >= decreaseTime)
            {
                CancleDecreaseDebuff();
            }
            else
            {
                decreaseTimer += Time.deltaTime;
            }
        }
    }

    // 控制怪物的转向
    private void Turning()
    {
        if (roadIndex + 1 < monsterPointList.Count)
        {
            float offsetX = monsterPointList[roadIndex].x - monsterPointList[roadIndex - 1].x;
            if (offsetX > 0) // 向右走
            {
                transform.eulerAngles = Vector3.zero;
            }
            else if (offsetX < 0) // 向左走
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
    }

    // 初始化怪物的一些属性
    private void InitMonster()
    {
        monsterID = 0;
        hp = 0;
        currentHP = 0;
        moveSpeed = 0;
        initMoveSpeed = 0;
        roadIndex = 1;
        reachCarrot = false;
        hpSlider.value = 1;
        hpSlider.gameObject.SetActive(false);
        prize = 0;
        transform.eulerAngles = Vector3.zero;
        CancleDecreaseDebuff();
    }

    // 承受伤害的方法
    private void TakeDamage(int value)
    {
        hpSlider.gameObject.SetActive(true);
        currentHP -= value;
        if (currentHP <= 0)
        {
            // 播放死亡音效
            gameController.PlayAudioEffect("NormalMordel/Monster/" + gameController.currentStage.bigLevelID + "/" + monsterID);
            DestroyMonster();
            return;
        }
        hpSlider.value = (float)currentHP / hp;
    }

    // 受到减速buff的效果
    private void DecreaseDebuff(BulletProperty bulletProperty)
    {
        if (!hasDecreaseSpeed) // 没有减速才受到buff
        {
            moveSpeed -= bulletProperty.debuffValue;
            shit.SetActive(true);
        }
        decreaseTimer = 0;
        decreaseTime = bulletProperty.debuffTime;
        hasDecreaseSpeed = true;
    }

    // 取消减速buff
    private void CancleDecreaseDebuff()
    {
        decreaseTimer = 0;
        decreaseTime = 0;
        hasDecreaseSpeed = false;
        moveSpeed = initMoveSpeed;
        shit.SetActive(false);
    }

    // 销毁怪物的方法
    private void DestroyMonster()
    {
        if (gameController.targetTrans == transform)
        {
            gameController.HideSignal();
        }

        if (!reachCarrot) // 被玩家杀死
        {
            // 生成金币
            GameObject coin = gameController.GetItem("CoinCanvas");
            coin.transform.Find("Img_Coin").GetComponent<CoinMove>().prize = prize;
            coin.transform.Find("Img_Coin").GetComponent<CoinMove>().ShowCoin();
            coin.transform.SetParent(gameController.transform);
            coin.transform.position = transform.position;
            // 增加玩家金币数量
            gameController.ChangeCoinNum(prize);
            // 概率生成奖励物品
            if (Random.Range(0, 50) == 0)
            {
                gameController.PlayAudioEffect("NormalMordel/GiftCreate");
                GameObject go = gameController.GetItem("Prize");
                go.transform.SetParent(gameController.transform);
                go.transform.position = transform.position;
            }
        }
        //TODO 产生销毁特效
        GameObject destroyEffect = gameController.GetItem("DestroyEffect");
        destroyEffect.transform.SetParent(gameController.transform);
        destroyEffect.transform.position = transform.position;

        gameController.killMonsterNum++;
        gameController.killMonsterTotalNum++;
        InitMonster();
        // 将怪物放进对象池
        gameController.PushItem("Monster", gameObject);
    }

    public void GetMonsterProperty()
    {
        runtimeAnimator = gameController.controllers[monsterID - 1];
        animator.runtimeAnimatorController = runtimeAnimator;
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
}
