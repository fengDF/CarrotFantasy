using UnityEngine;

/// <summary>
/// 塔的特异性属性脚本
/// </summary>
public class TowerPersonalProperty : MonoBehaviour
{
    // 引用
    [HideInInspector] public Tower tower;
    [HideInInspector] public Transform targetTrans; // 攻击目标
    protected Animator animator;
    protected GameController gameController;

    // 塔的属性值
    public int towerLevel;
    public float attackCD; // 攻击的CD
    [HideInInspector] public int price; // 塔自身的价格
    [HideInInspector] public int upLevelPrice; // 升级价格
    [HideInInspector] public int sellPrice; // 出售价格

    protected float attackTimer; // 攻击的计时器

    // 资源
    protected GameObject bullet;

    private void OnEnable()
    {
        gameController = GameController.Instance;
    }

    protected virtual void Start()
    {
        gameController = GameController.Instance;
        price = gameController.towerPriceList[tower.towerID];
        InitPrice();
        animator = transform.Find("tower").GetComponent<Animator>();
        attackTimer = attackCD;
    }

    protected virtual void Update()
    {
        if (gameController.isPause || targetTrans == null || gameController.gameOver) return;
        if (!targetTrans.gameObject.activeSelf)
        {
            targetTrans = null;
            return;
        }

        // 攻击
        if (attackTimer >= attackCD / gameController.gameSpeed)
        {
            attackTimer = 0;
            Attack();
        }
        else
        {
            attackTimer += Time.deltaTime;
        }

        // 旋转
        if (targetTrans.tag == StringManager.T_Item)
        {
            transform.LookAt(targetTrans.position + new Vector3(0, 0, 3));
        }
        else
        {
            transform.LookAt(targetTrans.position);
        }
        if (transform.eulerAngles.y == 0)
        {
            Vector3 euler = transform.eulerAngles;
            euler.y = 90;
            transform.eulerAngles = euler;
        }
    }

    public void Init()
    {
        tower = null;
    }

    // 初始化塔的各种价格
    private void InitPrice()
    {
        upLevelPrice = price * towerLevel * (towerLevel + 1) / 3;
        sellPrice = price * towerLevel / 3;
    }

    // 升级塔
    public void UpLevelTower()
    {
        gameController.PlayAudioEffect("NormalMordel/Tower/TowerUpdata");
        gameController.ChangeCoinNum(-upLevelPrice);
        GameObject effect = gameController.GetItem("UpLevelEffect"); // 产生升级特效
        effect.transform.SetParent(gameController.transform);
        effect.transform.position = transform.position;
        DestroyTower();
    }

    protected virtual void DestroyTower()
    {
        tower.DestroyTower();
    }

    protected virtual void Attack()
    {
        if (targetTrans == null) return;
        animator.Play("Attack"); // 播放攻击动画
        gameController.PlayAudioEffect("NormalMordel/Tower/Attack/" + tower.towerID);
        bullet = gameController.GetItem("Tower/ID" + tower.towerID + "/Bullet/" + towerLevel);
        bullet.transform.position = transform.position;
        bullet.transform.SetParent(gameController.transform);
        bullet.GetComponent<Bullet>().targetTrans = targetTrans;
    }

    public void SellTower()
    {
        gameController.PlayAudioEffect("NormalMordel/Tower/TowerSell");
        gameController.ChangeCoinNum(sellPrice); // 增加金币
        GameObject effect = gameController.GetItem("BuildEffect"); // 产生销毁特效
        effect.transform.SetParent(gameController.transform);
        effect.transform.position = transform.position;
        DestroyTower();
    }
}
