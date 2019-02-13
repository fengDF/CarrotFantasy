using UnityEngine;

/// <summary>
/// 星星塔的特异性属性脚本
/// </summary>
public class Star : TowerPersonalProperty
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
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
    }
}
