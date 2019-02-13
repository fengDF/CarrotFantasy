using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

/// <summary>
/// 地图格子类,存储格子信息以及建造在格子上的塔的信息,处理玩家与格子之间的交互
/// </summary>
public class GridPoint : MonoBehaviour
{
    /// <summary>
    /// 格子状态
    /// </summary>
    public struct GridState
    {
        public bool canBuild; // 可以建造
        public bool isMonsterPoint; // 是否时怪物路点
        public bool hasItem; // 是否有道具
        public int itemID; // 道具ID 
    }

    /// <summary>
    /// 格子索引
    /// </summary>
    public struct GridIndex
    {
        public int xIndex;
        public int yIndex;

        public GridIndex(int x, int y)
        {
            xIndex = x;
            yIndex = y;
        }
    }
#if Tool
    private Sprite monsterPointSprite; // 怪物路点图片资源
    public GameObject[] items;
    public GameObject currentItem;
#endif
    // 格子属性
    public GridState gridState;
    public GridIndex gridIndex;
    // 引用
    private GameController gameController;
    private SpriteRenderer spriteRenderer;
    private BoxCollider col;
    // 建塔相关属性
    [HideInInspector] public bool hasTower; // 该格子是否有塔
    private GameObject towerList; // 建塔列表的
    private GameObject towerCanvas; // 塔升级的画布
    private Transform upLevelButton;
    private Transform sellTowerButton;
    private Vector3 upLevelPos;
    private Vector3 sellPos;
    // 有塔之后相关属性
    public GameObject towerGO; // 塔
    public Tower tower;
    public TowerPersonalProperty towerPersonalProperty;
    private GameObject levelUpSignal; // 可以升级塔的信号图标


    // 资源
    private Sprite gridSprite; // 格子图片资源
    private Sprite startSprite; // 开始格子的图片资源
    private Sprite cantBuildSprite; // 不可以建造的图片资源

    private void Awake()
    {
#if Tool
        gridSprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/Grid");
        monsterPointSprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/1/Monster/6-1");
        items = new GameObject[10];
        string prefabsPath = "Prefabs/Game/" + MapMaker.Instance.bigLevelID + "/Item/";
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = Resources.Load<GameObject>(prefabsPath + i); 
            if(items [i] == null)
            {
                Debug.Log(prefabsPath + i);
            }
        }
#endif
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitGridState();
#if Game
        col = GetComponent<BoxCollider>();
        col.enabled = false;

        gameController = GameController.Instance;
        gridSprite = gameController.GetSprite("NormalMordel/Game/Grid");
        startSprite = gameController.GetSprite("NormalMordel/Game/StartSprite");
        cantBuildSprite = gameController.GetSprite("NormalMordel/Game/cantBuild");

        // 格子进场动画
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = startSprite;
        Tween t = DOTween.To(() => spriteRenderer.color, toColor => spriteRenderer.color = toColor, new Color(1, 1, 1, 0), 3f);
        t.OnComplete(ChangeSpriteToGrid);

        // 建塔相关的属性赋值
        towerList = gameController.towerListCanvas;
        towerCanvas = gameController.towerCanvas;
        upLevelButton = towerCanvas.transform.Find("Btn_UpLevel");
        sellTowerButton = towerCanvas.transform.Find("Btn_Sell");
        upLevelPos = upLevelButton.localPosition;
        sellPos = sellTowerButton.localPosition;

        levelUpSignal = transform.Find("UpLevelSignal").gameObject;
        levelUpSignal.SetActive(false);
#endif
    }

    private void Update()
    {
        if (levelUpSignal != null)
        {
            if (hasTower)
            {
                if (towerPersonalProperty.upLevelPrice <= gameController.Coin && towerPersonalProperty.towerLevel < 3) // 能升级
                {
                    levelUpSignal.SetActive(true);
                }
                else
                {
                    levelUpSignal.SetActive(false);
                }
            }
            else // 没塔
            {
                if (levelUpSignal.activeSelf)
                {
                    levelUpSignal.SetActive(false);
                }
            }
        }
    }

    // 将格子改回原来的渲染
    private void ChangeSpriteToGrid()
    {
        col.enabled = true;
        spriteRenderer.enabled = false;
        spriteRenderer.color = new Color(1, 1, 1, 1);

        if (gridState.canBuild)
        {
            spriteRenderer.sprite = gridSprite;
        }
        else
        {
            spriteRenderer.sprite = cantBuildSprite;
        }
    }

    // 初始化格子状态
    public void InitGridState()
    {
        gridState.canBuild = true;
        gridState.isMonsterPoint = false;
        gridState.hasItem = false;
        gridState.itemID = -1;
        spriteRenderer.enabled = false;
#if Tool
        spriteRenderer.sprite = gridSprite;
        Destroy(currentItem);
#elif Game
        towerGO = null;
        tower = null;
        towerPersonalProperty = null;
        hasTower = false;
#endif
    }

#if Game
    // 更新格子状态的方法
    public void UpdateGrid()
    {
        if (gridState.canBuild)
        {
            spriteRenderer.enabled = true;
            if (gridState.hasItem)
            {
                CreateItem();
            }
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    // 创建道具
    private void CreateItem()
    {
        GameObject item = GameController.Instance.GetItem(GameController.Instance.currentStage.bigLevelID + "/Item/" + gridState.itemID);
        item.transform.SetParent(GameController.Instance.transform);
        Vector3 pos = transform.position - new Vector3(0, 0, 3);
        if (gridState.itemID <= 2) pos += new Vector3(GameController.Instance.mapMaker.gridWidth / 2, -GameController.Instance.mapMaker.gridHeight / 2);
        else if (gridState.itemID <= 4) pos.x += GameController.Instance.mapMaker.gridWidth / 2;
        item.transform.position = pos;
        item.GetComponent<Item>().gridPoint = this;
        item.GetComponent<Item>().itemID = gridState.itemID;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;// 如果点击到的是UI
        gameController.HandleGrid(this);
    }

    // 成功建塔之后的处理方法
    public void AfterBuild()
    {
        spriteRenderer.enabled = false;
        hasTower = true;
        // 获取塔的信息
        towerGO = transform.GetChild(1).gameObject;
        tower = towerGO.GetComponent<Tower>();
        towerPersonalProperty = towerGO.GetComponent<TowerPersonalProperty>();
    }

    // 纠正建塔列表的位置
    private Vector3 CorrectTowerPos()
    {
        Vector3 correctPos = Vector3.zero;
        if (gridIndex.xIndex <= 3 && gridIndex.xIndex >= 0) // 左边缘
        {
            correctPos += new Vector3(gameController.mapMaker.gridWidth * (3 - gridIndex.xIndex) / 3, 0, 0);
        }
        else if (gridIndex.xIndex <= 11 && gridIndex.xIndex >= 8)
        {
            correctPos += new Vector3(gameController.mapMaker.gridWidth * (8 - gridIndex.xIndex) / 3, 0, 0);
        }

        if (gridIndex.yIndex < 4)
        {
            correctPos += new Vector3(0, gameController.mapMaker.gridHeight, 0);
        }
        else
        {
            correctPos += new Vector3(0, -gameController.mapMaker.gridHeight, 0);
        }

        return correctPos;
    }

    // 纠正画布位置的方法
    private void CorrectHandleTowerPos()
    {
        upLevelButton.localPosition = Vector3.zero;
        sellTowerButton.localPosition = Vector3.zero;
        if (gridIndex.yIndex == 0)
        {
            if (gridIndex.xIndex == 0)
            {
                sellTowerButton.position += new Vector3(gameController.mapMaker.gridWidth * 3 / 4, 0, 0);
            }
            else
            {
                sellTowerButton.position -= new Vector3(gameController.mapMaker.gridWidth * 3 / 4, 0, 0);
            }
            upLevelButton.localPosition = upLevelPos;
        }
        else if (gridIndex.yIndex >= 6)
        {
            if (gridIndex.xIndex == 0)
            {
                upLevelButton.position += new Vector3(gameController.mapMaker.gridWidth * 3 / 4, 0, 0);
            }
            else
            {
                upLevelButton.position -= new Vector3(gameController.mapMaker.gridWidth * 3 / 4, 0, 0);
            }
            sellTowerButton.localPosition = sellPos;
        }
        else
        {
            upLevelButton.localPosition = upLevelPos;
            sellTowerButton.localPosition = sellPos;
        }
    }

    // 显示格子的内容
    public void ShowGrid()
    {
        if (!hasTower)
        {
            spriteRenderer.enabled = true;
            // 显示建塔列表
            towerList.transform.position = CorrectTowerPos() + transform.position;
            towerList.SetActive(true);
        }
        else
        {
            towerCanvas.transform.position = transform.position;
            CorrectHandleTowerPos();
            towerCanvas.SetActive(true);
            // 显示塔的攻击范围
            towerGO.transform.Find("attackRange").gameObject.SetActive(true);
        }
    }

    //隐藏格子的内容
    public void HideGrid()
    {
        if (!hasTower)
        {
            // TODO 隐藏建塔列表
            towerList.SetActive(false);
        }
        else
        {
            towerCanvas.SetActive(false);
            towerGO.transform.Find("attackRange").gameObject.SetActive(false);
        }
        spriteRenderer.enabled = false;
    }

    // 显示该格子不能建塔
    public void ShowCantBuild()
    {
        spriteRenderer.enabled = true;
        Tween t = DOTween.To(() => spriteRenderer.color, toColor => spriteRenderer.color = toColor, new Color(1, 1, 1, 0), 2f);
        t.OnComplete(() =>
        {
            spriteRenderer.enabled = false;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        });
    }

#endif
#if Tool
    private void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.P)) // 怪物路点
        {
            gridState.canBuild = false;
            spriteRenderer.enabled = true;
            gridState.isMonsterPoint = !gridState.isMonsterPoint;
            if (gridState.isMonsterPoint) // 是怪物路点
            {
                MapMaker.Instance.monsterPointList.Add(gridIndex);
                spriteRenderer.sprite = monsterPointSprite;
            }
            else
            {
                MapMaker.Instance.monsterPointList.Remove(gridIndex);
                spriteRenderer.sprite = gridSprite;
                gridState.canBuild = true;
            }
        }
        else if (Input.GetKey(KeyCode.I)) // 道具
        {
            gridState.itemID++;
            if(gridState.itemID == items.Length)
            {
                gridState.itemID = -1;
                Destroy(currentItem);
                gridState.hasItem = false;
                return;
            }
            if(currentItem == null)
            {
                // 产生道具
                CreateItem();
            }
            else
            {
                Destroy(currentItem);
                // 产生道具
                CreateItem();
            }
            gridState.hasItem = true;
        }
        else if(!gridState.isMonsterPoint)
        {
            gridState.canBuild = !gridState.canBuild;

            if (gridState.canBuild)
            {
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }

    // 生成道具的方法
    private void CreateItem()
    {
        Vector3 pos = transform.position;
        if(gridState.itemID <= 2)
        {
            pos += new Vector3(MapMaker.Instance.gridWidth/2, -MapMaker.Instance.gridHeight/2);
        }else if(gridState.itemID == 3 || gridState.itemID == 4)
        {
            pos += new Vector3(MapMaker.Instance.gridWidth/2,0);
        }
        GameObject item = Instantiate(items[gridState.itemID], pos, Quaternion.identity);
        currentItem = item;
    }

    public void UpdateGrid()
    {
        if (gridState.canBuild)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = gridSprite;
            if (gridState.hasItem)
            {
                CreateItem();
            }
        }
        else
        {
            if (gridState.isMonsterPoint)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.sprite = monsterPointSprite;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
    }
#endif
}

