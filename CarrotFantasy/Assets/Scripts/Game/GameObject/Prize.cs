using UnityEngine;

public class Prize : MonoBehaviour
{
    private void Update()
    {
        if (GameController.Instance.gameOver)
        {
            GameController.Instance.PushItem("Prize", gameObject);
        }
    }

    private void OnMouseDown()
    {
        GameController.Instance.PlayAudioEffect("NormalMordel/GiftGot");
        GameController.Instance.ShowPrizePage(gameObject);
    }
}
