using UnityEngine;
using UnityEngine.UI;

public class ButtonUpLevel : MonoBehaviour
{
    private int price;

    private GameController gameController;
    private Button button;
    private Text text;
    private Image image;

    private Sprite canUpLevelSprite; // 可以升级的图片资源
    private Sprite cantUpLevelSprite; // 没钱升级的图片资源
    private Sprite reachHighSprite; // 到达顶级的图片资源

    private void Start()
    {
        gameController = GameController.Instance;
        button = GetComponent<Button>();
        button.onClick.AddListener(UpLevel);
        text = transform.Find("Txt_UpLevel").GetComponent<Text>();
        image = GetComponent<Image>();

        canUpLevelSprite = gameController.GetSprite("NormalMordel/Game/Tower/Btn_CanUpLevel");
        cantUpLevelSprite = gameController.GetSprite("NormalMordel/Game/Tower/Btn_CantUpLevel");
        reachHighSprite = gameController.GetSprite("NormalMordel/Game/Tower/Btn_ReachHighestLevel");

        transform.parent.gameObject.SetActive(false); // 刚开始取消显示,以便下次调用显示方法时执行OnEnable更新UI显示
    }

    private void OnEnable()
    {
        if (text == null) return; // 如果还没有执行Start(第一次进入场景时) 则不更新UI显示
        UpdateUI();
    }

    // 更新价格的UI显示
    private void UpdateUI()
    {
        if (gameController.selectGrid.towerPersonalProperty.towerLevel == 3)
        {
            image.sprite = reachHighSprite;
            button.interactable = false;
            text.enabled = false;
        }
        else
        {
            text.enabled = true;
            price = gameController.selectGrid.towerPersonalProperty.upLevelPrice;
            text.text = price.ToString();
            if (gameController.Coin >= price)
            {
                button.interactable = true;
                image.sprite = canUpLevelSprite;
            }
            else
            {
                button.interactable = false;
                image.sprite = cantUpLevelSprite;
            }
        }
    }

    // 升级的方法
    private void UpLevel()
    {
        // 赋值建造者需要的属性
        gameController.towerBuilder.towerID = gameController.selectGrid.tower.towerID;
        gameController.towerBuilder.towerLevel = gameController.selectGrid.towerPersonalProperty.towerLevel + 1;
        // 销毁之前的塔
        gameController.selectGrid.towerPersonalProperty.UpLevelTower();
        // 产生新塔
        GameObject towerGO = gameController.towerBuilder.GetProduct();
        towerGO.transform.SetParent(gameController.selectGrid.transform);
        towerGO.transform.localPosition = Vector3.zero;
        gameController.selectGrid.AfterBuild();
        gameController.selectGrid.HideGrid();
        gameController.selectGrid = null;
    }
}
