using UnityEngine;

public class CrystalBullet : Bullet
{
    private readonly float attackTime = 0.65f; // 减血的间隔时间
    private float attackTimer; // 减血的计时器
    private bool canTakeDamage;

    protected override void Update()
    {
        if (GameController.Instance.gameOver)
        {
            DestroyBullet();
            return;
        }

        if (GameController.Instance.isPause) return;

        if (targetTrans == null || !targetTrans.gameObject.activeSelf)
        {
            DestroyBullet();
            return;
        }

        // 子弹的移动与转向
        if (targetTrans.tag == StringManager.T_Item) // 如果是道具
        {           
            transform.LookAt(targetTrans.position + new Vector3(0, 0, 3)); // 子弹转向目标
        }
        else
        {
            transform.LookAt(targetTrans.position); // 子弹转向目标
        }

        if (!canTakeDamage)
        {
            attackTimer += Time.deltaTime;
            if(attackTimer >= attackTime - towerLevel * 0.15f)
            {
                GameController.Instance.PlayAudioEffect("NormalMordel/Tower/Attack/5");
                canTakeDamage = true;
                attackTimer = 0;
                DecreaseHp();
            }
        }
    }

    private void DecreaseHp()
    {
        if (!canTakeDamage) return;
        if (targetTrans != null && targetTrans.gameObject.activeSelf)
        {
            if(targetTrans.tag == StringManager.T_Monster ||(targetTrans.tag == StringManager.T_Item && StringManager.T_Item == GameController.Instance.targetTrans.tag))
            {
                targetTrans.SendMessage("TakeDamage", attackValue);
                CreateEffect();
                canTakeDamage = false;
            }
        }
    }

    protected override void CreateEffect()
    {
        if (targetTrans == null) return;
        GameObject effect = GameController.Instance.GetItem("Tower/ID" + towerID + "/Effect/" + towerLevel);
        effect.transform.SetParent(GameController.Instance.transform);
        effect.transform.position = targetTrans.position;
    }
}
