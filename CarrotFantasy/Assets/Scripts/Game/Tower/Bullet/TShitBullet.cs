using UnityEngine;

public class TShitBullet : Bullet
{
    private BulletProperty bulletProperty;

    void Start()
    {
        bulletProperty = new BulletProperty
        {
            debuffTime = towerLevel * 0.5f,
            debuffValue = (towerLevel * (towerLevel - 1)) * 0.1f + 0.25f
        };
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == StringManager.T_Monster || collision.tag == StringManager.T_Item)
        {
            if (collision.gameObject.activeSelf && collision.transform == targetTrans)
            {
                if(collision.tag == StringManager.T_Monster) // 触发减速方法
                {
                    collision.SendMessage("DecreaseDebuff", bulletProperty);
                }
                collision.SendMessage("TakeDamage", attackValue);
                CreateEffect();
                DestroyBullet();
            }
        }
    }
}
