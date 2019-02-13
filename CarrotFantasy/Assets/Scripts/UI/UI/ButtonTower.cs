using UnityEngine;
using UnityEngine.UI;

public class ButtonTower : MonoBehaviour
{
    [HideInInspector] public int towerID;
    [HideInInspector] public int price;

    private Button btn;
    private Sprite canClickSprite;
    private Sprite cantClickSprite;
    private Image towerImg;
    private GameController gameController;

    private void OnEnable()
    {
        if (price == 0) return;
        UpdateIcon(); 
    }

    private void Start()
    {
        gameController = GameController.Instance;
        towerImg = GetComponent<Image>();
        btn = GetComponent<Button>();

        canClickSprite = gameController.GetSprite("NormalMordel/Game/Tower/" + towerID + "/CanClick1");
        cantClickSprite = gameController.GetSprite("NormalMordel/Game/Tower/" + towerID + "/CanClick0");

        price = gameController.towerPriceList[towerID];
        UpdateIcon();

        btn.onClick.AddListener(BuildTower);
    }

    // 更新图标的方法
    private void UpdateIcon()
    {
        if (gameController.Coin >= price)
        {
            towerImg.sprite = canClickSprite;
            btn.interactable = true;
        }
        else
        {
            towerImg.sprite = cantClickSprite;
            btn.interactable = false;
        }
    }

    private void BuildTower()
    {
        gameController.PlayAudioEffect("NormalMordel/Tower/TowerBulid");
        // 由建造者去建塔
        gameController.towerBuilder.towerID = towerID;
        gameController.towerBuilder.towerLevel = 1;
        GameObject tower = gameController.towerBuilder.GetProduct();
        tower.transform.SetParent(gameController.selectGrid.transform);
        tower.transform.localPosition = Vector3.zero;

        // 产生建造特效
        GameObject buildEffect = gameController.GetItem("BuildEffect");
        buildEffect.transform.SetParent(gameController.transform);
        buildEffect.transform.position = gameController.selectGrid.transform.position;

        // 处理格子完成建造的方法
        gameController.selectGrid.HideGrid();
        gameController.selectGrid.AfterBuild();

        gameController.ChangeCoinNum(-price);
        gameController.selectGrid = null;
    }
}