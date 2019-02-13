using UnityEngine;
using UnityEngine.UI;

public class Carrot : MonoBehaviour
{
    // 萝卜不同状态的图片
    private Sprite[] carrotSprites;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Text text;

    private float idleTimer; // 萝卜Idle动画的计时器

    private void Awake()
    {
        carrotSprites = new Sprite[7];
        for(int i = 0; i < carrotSprites.Length; i++)
        {
            carrotSprites[i] = GameController.Instance.GetSprite("NormalMordel/Game/Carrot/" + i);
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        text = transform.Find("HpCanvas/Image/Text").GetComponent<Text>();
    }

    private void Update()
    {
        if (animator.enabled)
        {
            if (GameController.Instance.carrotHP < 10)
            {
                animator.enabled = false;
            }

            if (idleTimer >= 20)
            {
                animator.Play("CarrotIdle");
                idleTimer = 0;
            }
            else
            {
                idleTimer += Time.deltaTime;
            }
        }
    }

    private void OnMouseDown()
    {
        if(GameController.Instance.carrotHP == 10)
        {
            GameController.Instance.PlayAudioEffect("NormalMordel/Carrot/" + Random.Range(1, 4));
            animator.Play("CarrotTouch");
        }
    }

    // 更新血量和图片显示
    public void UpdateHpUI()
    {
        int hp = GameController.Instance.carrotHP;
        if(hp <= 0)
        {
            GameController.Instance.GameOver();
        }
        if (hp < 10) animator.enabled = false;
        text.text = hp.ToString();

        if(hp < 10 && hp >= 7)
        {
            spriteRenderer.sprite = carrotSprites[6];
        }
        else if(hp > 0 && hp <= 6)
        {
            spriteRenderer.sprite = carrotSprites[hp-1];
        }
    }
}
