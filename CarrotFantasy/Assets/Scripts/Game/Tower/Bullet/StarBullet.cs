using UnityEngine;

public class StarBullet : MonoBehaviour
{
    public int attackValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.activeSelf && (collision.tag == StringManager.T_Monster || collision.tag == StringManager.T_Item))
        {
            collision.SendMessage("TakeDamage", attackValue);
        }
    }
}
