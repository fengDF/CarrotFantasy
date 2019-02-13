using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public Transform targetTrans;
    public int moveSpeed; // 移动速度
    public int attackValue;
    public int towerLevel;
    public int towerID;

    protected virtual void Update()
    {
        if (GameController.Instance.gameOver)
        {
            DestroyBullet();
            return;
        }

        if (GameController.Instance.isPause) return;

        if(targetTrans == null || !targetTrans.gameObject.activeSelf)
        {
            DestroyBullet();
            return;
        }

        // 子弹的移动与转向
        if(targetTrans.tag == StringManager.T_Item) // 如果是道具
        {
            transform.position = Vector3.Lerp(transform.position, targetTrans.position + new Vector3(0, 0, 3),
                moveSpeed / Vector3.Distance(transform.position, targetTrans.position + new Vector3(0, 0, 3)) * Time.deltaTime * GameController.Instance.gameSpeed);
            transform.LookAt(targetTrans.position + new Vector3(0, 0, 3)); // 子弹转向目标
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetTrans.position,
               moveSpeed / Vector3.Distance(transform.position, targetTrans.position) * Time.deltaTime * GameController.Instance.gameSpeed);
            transform.LookAt(targetTrans.position); // 子弹转向目标
        }
    }

    // 销毁子弹
    protected virtual void DestroyBullet()
    {
        targetTrans = null;
        GameController.Instance.PushItem("Tower/ID" + towerID + "/Bullet/" + towerLevel,gameObject);
    }

    // 创建子弹命中特效
    protected virtual void CreateEffect()
    {
        GameObject effect = GameController.Instance.GetItem("Tower/ID" + towerID + "/Effect/" + towerLevel);
        effect.transform.position = transform.position;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == StringManager.T_Monster || collision.tag == StringManager.T_Item)
        {
            if (collision.gameObject.activeSelf && collision.transform == targetTrans)
            {
                collision.SendMessage("TakeDamage", attackValue);
                CreateEffect();
                DestroyBullet();
            }
        }
    }
}

/// <summary>
/// 子弹的buff属性
/// </summary>
public struct BulletProperty
{
    public float debuffTime; // buff时间
    public float debuffValue; // buff值
}