using UnityEngine;

public class WindmillBullet : Bullet
{
    public float existTime; // 子弹存活时间

    private bool hasTarget; // 是否拥有目标
    private float existTimer; // 子弹存在的计时器

    private void OnEnable()
    {
        hasTarget = false;
        existTimer = 0;
    }

    // 初始化目标的方法
    private void InitTarget()
    {
        if(targetTrans.tag == StringManager.T_Item)
        {
            transform.LookAt(targetTrans.position + new Vector3(0, 0, 3));
        }
        else
        {
            transform.LookAt(targetTrans.position);
        }
        if(transform.eulerAngles.y == 0)
        {
            transform.eulerAngles += new Vector3(0, 90, 0);
        }
    }

    protected override void Update()
    {
        if (GameController.Instance.gameOver || existTimer >= existTime)
        {
            DestroyBullet();
            return;
        }
        if (GameController.Instance.isPause) return;

        existTimer += Time.deltaTime;

        if (hasTarget)
        {
            transform.Translate(transform.forward * moveSpeed * Time.deltaTime * GameController.Instance.gameSpeed,Space.World);
        }
        else
        {
            if(targetTrans != null && targetTrans.gameObject.activeSelf)
            {
                hasTarget = true;
                InitTarget();
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == StringManager.T_Monster || collision.tag == StringManager.T_Item)
        {
            if (collision.gameObject.activeSelf)
            {
                collision.SendMessage("TakeDamage", attackValue);
                CreateEffect();
            }
        }
    }
}
