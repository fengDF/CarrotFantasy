using UnityEngine;

public class Crystal : TowerPersonalProperty
{
    private float distance;
    private float bulletLength;
    private float bulletWidth;

    private void OnEnable()
    {
        if (animator == null) return;
        bullet = gameController.GetItem("Tower/ID" + tower.towerID + "/Bullet/" + towerLevel);
        bullet.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        bullet = gameController.GetItem("Tower/ID" + tower.towerID + "/Bullet/" + towerLevel);
        bullet.SetActive(false);
    }

    protected override void Update()
    {
        if (gameController.isPause || targetTrans == null || gameController.gameOver)
        {
            if (targetTrans == null)
            {
                bullet.SetActive(false);
            }
            return;
        }
        if (!targetTrans.gameObject.activeSelf)
        {
            targetTrans = null;
            return;
        }
        Attack();
    }

    protected override void Attack()
    {
        animator.Play("Attack");
        if (targetTrans.tag == StringManager.T_Item)
        {
            distance = Vector3.Distance(transform.position, targetTrans.position + new Vector3(0, 0, 3));
        }
        else
        {
            distance = Vector3.Distance(transform.position, targetTrans.position);
        }

        bulletWidth = 3 / distance;
        bulletLength = distance / 2 - distance * 0.1f;
        bulletWidth = Mathf.Clamp(bulletWidth, 0.5f, 1);
        bullet.transform.position = new Vector3((transform.position.x + targetTrans.position.x) / 2, (transform.position.y + targetTrans.position.y) / 2);
        bullet.transform.localScale = new Vector3(1, bulletWidth, bulletLength);
        bullet.SetActive(true);
        bullet.GetComponent<Bullet>().targetTrans = targetTrans;
    }

    protected override void DestroyTower()
    {
        gameController.PushItem("Tower/ID" + tower.towerID + "/Bullet/" + towerLevel, bullet);
        bullet = null;
        base.DestroyTower();
    }
}
