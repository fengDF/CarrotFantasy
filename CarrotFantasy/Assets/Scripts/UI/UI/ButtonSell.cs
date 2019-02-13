using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 卖塔按钮
/// </summary>
public class ButtonSell : MonoBehaviour
{
    private int price;
    private Button button;
    private Text text;
    private GameController gameController;

    private void OnEnable()
    {
        if (gameController == null) return;
        price = gameController.selectGrid.towerPersonalProperty.sellPrice;
        text.text = price.ToString();
    }

    private void Start()
    {
        gameController = GameController.Instance;
        button = GetComponent<Button>();
        text = transform.Find("Txt_Sell").GetComponent<Text>();
        button.onClick.AddListener(SellTower);
    }

    private void SellTower()
    {
        gameController.selectGrid.towerPersonalProperty.SellTower();
        gameController.selectGrid.HideGrid();
        gameController.selectGrid.InitGridState();
        gameController.selectGrid = null;
    }
}
