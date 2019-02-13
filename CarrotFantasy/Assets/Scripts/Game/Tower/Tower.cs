using UnityEngine;

/// <summary>
/// 塔的共同特性脚本
/// </summary>
public class Tower : MonoBehaviour
{
    [HideInInspector] public int towerID;

    private CircleCollider2D circleCollider; // 塔的攻击范围触发检测
    private TowerPersonalProperty towerPersonalProperty; // 塔的特异性脚本
    private SpriteRenderer attackRange; // 塔的攻击范围渲染

    [HideInInspector] public bool isTarget; // 是集火目标
    [HideInInspector] public bool hasTarget; // 攻击范围内有集火目标

    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (GameController.Instance.isPause || GameController.Instance.gameOver) return;

        if (isTarget)
        {
            // 有集火目标但是不是当前的集火目标时
            if(towerPersonalProperty.targetTrans != GameController.Instance.targetTrans)
            {
                towerPersonalProperty.targetTrans = null;
                hasTarget = isTarget = false;
            }
        }

        if (hasTarget)
        {
            // 有目标但是当前目标已经死亡时
            if (!towerPersonalProperty.targetTrans.gameObject.activeSelf)
            {
                towerPersonalProperty.targetTrans = null;
                hasTarget = isTarget = false;
            }
        }
    }

    // 初始化塔属性的方法
    private void Init()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        towerPersonalProperty = GetComponent<TowerPersonalProperty>();
        towerPersonalProperty.tower = this;
        attackRange = transform.Find("attackRange").GetComponent<SpriteRenderer>();
        attackRange.gameObject.SetActive(false);
        attackRange.transform.localScale = Vector3.one * towerPersonalProperty.towerLevel;
        circleCollider.radius = 1.1f * towerPersonalProperty.towerLevel;
        //circleCollider.radius = 5.5f; // 开发测试数据
        isTarget = false;
        hasTarget = false;
    }

    public void GetTowerProperty()
    {

    }

    // 销毁塔的方法
    public void DestroyTower()
    {
        towerPersonalProperty.Init();
        GameController.Instance.PushItem("Tower/ID" + towerID + "/TowerSet/" + towerPersonalProperty.towerLevel,gameObject);
    }

    #region 塔搜寻目标逻辑(当有怪物进入停留离开时)
    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.tag != StringManager.T_Monster && col.tag != StringManager.T_Item)/* && isTarget*/) return;
        Transform target = GameController.Instance.targetTrans;
        if(target!= null) // 有集火目标时
        {
            if (!isTarget) // 还没找到集火目标
            {
                // 是物品且是集火目标
                if(col.tag == StringManager.T_Item && col.transform == target)
                {
                    towerPersonalProperty.targetTrans = col.transform;
                    isTarget = hasTarget = true;
                }
                else if(col.tag == StringManager.T_Monster) // 是怪物
                {
                    // 且是集火目标
                    if(col.transform == target)
                    {
                        towerPersonalProperty.targetTrans = col.transform;
                        isTarget = hasTarget = true;
                    }
                    else if(col.transform != target && !hasTarget)// 且不是集火目标且没有目标
                    {
                        towerPersonalProperty.targetTrans = col.transform;
                        hasTarget = true;
                    }
                }
            }
        }
        else // 没有集火目标时
        {
            if(!hasTarget && col.tag == StringManager.T_Monster) // 没有目标且进入塔攻击范围的是怪物
            {
                towerPersonalProperty.targetTrans = col.transform;
                hasTarget = true;
            }
        }
     }

    private void OnTriggerStay2D(Collider2D col)
    {

        if ((col.tag != StringManager.T_Monster && col.tag != StringManager.T_Item)/* || isTarget*/) return;
        Transform target = GameController.Instance.targetTrans;
        if (target != null) // 有集火目标时
        {
            if (!isTarget) // 还没找到集火目标
            {
                // 是物品且是集火目标
                if (col.tag == StringManager.T_Item && col.transform == target)
                {
                    towerPersonalProperty.targetTrans = col.transform;
                    isTarget = hasTarget = true;
                }
                else if (col.tag == StringManager.T_Monster) // 是怪物
                {
                    // 且是集火目标
                    if (col.transform == target)
                    {
                        towerPersonalProperty.targetTrans = col.transform;
                        isTarget = hasTarget = true;
                    }
                    else if (col.transform != target && !hasTarget)// 且不是集火目标且没有目标
                    {
                        towerPersonalProperty.targetTrans = col.transform;
                        hasTarget = true;
                    }
                }
            }
        }
        else // 没有集火目标时
        {
            if (!hasTarget && col.tag == StringManager.T_Monster) // 没有目标且进入塔攻击范围的是怪物
            {
                towerPersonalProperty.targetTrans = col.transform;
                hasTarget = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if(towerPersonalProperty.targetTrans == col.transform) // 如果离开范围的是攻击目标
        {
            // 丢失目标的处理
            towerPersonalProperty.targetTrans = null;
            hasTarget = false;
            isTarget = false;
        }
    }
    #endregion
}
