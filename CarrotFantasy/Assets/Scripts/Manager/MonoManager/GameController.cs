using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏控制管理,负责控制游戏的整个逻辑
/// </summary>
public class GameController : MonoBehaviour
{
    // 引用
    public static GameController Instance { get; private set; } //  游戏控制单例
    public Level level;
    [HideInInspector] public MapMaker mapMaker;
    public Stage currentStage; // 当前关卡的信息
    [HideInInspector] public GridPoint selectGrid; // 上一个选中的格子
    [HideInInspector] public Transform targetTrans; // 集火目标
    public GameObject targetSignal; // 集火的信号
    private NormalModelPanel normalModelPanel; // 游戏的UI面板
    private GameManager mGameManager;

    // 产怪逻辑
    [HideInInspector] public int[] monstersID; // 当前波次的怪物列表
    private int currentMonsterIndex; // 当前怪物的索引(当前波次下的某一怪物)
    [HideInInspector] public bool creatingMonster; // 是否继续产怪

    [HideInInspector] public bool gameOver; // 游戏是否结束
    [HideInInspector] public bool isPause;

    // 游戏资源
    [HideInInspector] public RuntimeAnimatorController[] controllers;

    // 建造者
    private MonsterBuilder monsterBuilder;
    public TowerBuilder towerBuilder;

    // 建塔相关属性
    public Dictionary<int, int> towerPriceList; // 建塔价格表
    // [HideInInspector] public List<GameObject> towerList; // 建塔按钮的列表
    public GameObject towerCanvas; // 升级或者销售塔的画布
    public GameObject towerListCanvas; // 建塔按钮列表的画布

    // 用于计数的成员变量
    [HideInInspector] public int killMonsterNum; // 杀死的怪物数量(当前波次)
    [HideInInspector] public int clearItemNum; // 当前关卡玩家销毁的道具数量
    [HideInInspector] public int killMonsterTotalNum; // 杀怪总数(当前关卡)

    // 玩家属性值
    [HideInInspector] public int carrotHP = 10;
    public int Coin { get; private set; }
    [HideInInspector] public int gameSpeed; // 游戏速度(一倍速,二倍速)

    private void Awake()
    {
#if Game
        // 一些引用的赋值
        Instance = this;
        mGameManager = GameManager.Instance;
        currentStage = mGameManager.CurrentStage;
        normalModelPanel = mGameManager.UIManager.mUIFacade.currentScenePanelDict[StringManager.P_NormalModelPanel] as NormalModelPanel;
        mapMaker = GetComponent<MapMaker>();
        mapMaker.InitMapMaker();
        mapMaker.Loadmap(currentStage.bigLevelID, currentStage.levelID); // 加载地图
        level = new Level(mapMaker.roundInfoList.Count, mapMaker.roundInfoList); // 获取加载完地图中怪物波次的信息
        monsterBuilder = new MonsterBuilder();
        towerBuilder = new TowerBuilder();

        // 玩家属性的赋值
        gameSpeed = 1;
        Coin = 500;
        isPause = true;
        gameOver = false;

        normalModelPanel.EnterPanel();

        // RuntimeAnimatorController资源的赋值
        controllers = new RuntimeAnimatorController[12];
        for (int i = 0; i < controllers.Length; i++)
        {
            controllers[i] = GetRuntimeAnimator("Monster/" + currentStage.bigLevelID.ToString() + "/" + (i + 1));
        }

        // 建塔列表的赋值
        for (int i = 0; i < currentStage.towerIDList.Length; i++)
        {
            GameObject item = mGameManager.GetItem(FactoryType.UI, "Btn_TowerBuilder");
            item.GetComponent<ButtonTower>().towerID = currentStage.towerIDList[i];
            item.transform.SetParent(towerListCanvas.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }

        // 建塔列表的价格赋值
        towerPriceList = new Dictionary<int, int>()
        {
            {1,100 },
            {2,120 },
            {3,160 },
            {4,160 },
            {5,160 },
        };
#endif
    }

    private void Update()
    {
#if Game
        if (!isPause)
        {
            // 产生怪物的逻辑
            if (killMonsterNum == monstersID.Length) // 清除完该波怪
            {
                AddRoundNum(); // 添加怪物波次索引
            }
            else
            {
                // 继续产怪
                if (!creatingMonster)
                {
                    CreateMonster();
                }
            }
        }
        else
        {
            // 停止继续产生怪物
            if (creatingMonster)
            {
                StopCreateMonster();
                creatingMonster = false;
            }
        }
#endif
    }

    #region 游戏音效的处理

    public void PlayAudioEffect(string audioPath,float volume = 0.5f)
    {
        mGameManager.AudioManager.PlayEffectMusic(GetAudioClip(audioPath),volume);
    }

    #endregion

    #region 游戏逻辑相关的方法

    // 开始游戏的方法
    public void StartGame()
    {
        isPause = false;
        level.HandleRound();
    }

    // 游戏失败的方法
    public void GameOver()
    {
        gameOver = true;
        normalModelPanel.ShowGameOver();
    }

    // 游戏胜利的方法 
    public void Win()
    {
        gameOver = true;
        normalModelPanel.ShowGameWin();
    }

    // 显示最后一波怪的UI
    public void ShowFinalWave()
    {
        normalModelPanel.ShowFinalWave();
    }

    // 改变玩家金币数量
    public void ChangeCoinNum(int value)
    {
        Coin += value;
        // 更新UI显示
        normalModelPanel.topPage.UpdateCoinText();
    }

    // 改变萝卜的血量的方法
    public void ChangeCarrotHP(int value = -1)
    {
        if (value < 0) PlayAudioEffect("NormalMordel/Carrot/Crash");
        carrotHP += value;
        // 更新萝卜血量UI显示
        mapMaker.carrot.UpdateHpUI();
    }

    // 判断当前道具是否全部清除的方法
    public bool IsAllClearItem()
    {
        for(int x = 0;x < MapMaker.col; x++)
        {
            for(int y = 0;y < MapMaker.row; y++)
            {
                if (mapMaker.gridPoints[x, y].gridState.hasItem)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // 获得萝卜血量的状态
    public int GetCarrotHealth()
    {
        if (carrotHP == 10) return 1;
        else if (carrotHP < 10 && carrotHP >= 6) return 2;
        else if (carrotHP > 0 && carrotHP < 6) return 3;
        else return 0;
    }

    public void ShowPrizePage(GameObject prize)
    {
        isPause = true;
        normalModelPanel.ShowPrize();
        PushItem("Prize", prize);
    }

    // 显示集火信号的方法
    public void ShowSignal()
    {
        PlayAudioEffect("NormalMordel/Tower/ShootSelect");
        targetSignal.transform.position = targetTrans.position + new Vector3(0, mapMaker.gridHeight / 2,  0);
        targetSignal.transform.SetParent(targetTrans);
        targetSignal.SetActive(true);
    }

    // 隐藏集火信号的方法
    public void HideSignal()
    {
        targetSignal.SetActive(false);
        targetTrans = null;
    }

    // 处理格子的相关方法,参数是当前选择的格子
    public void HandleGrid(GridPoint grid)
    {
        if (grid.gridState.canBuild)
        {
            if (selectGrid == null) // 没有上一个格子
            {
                PlayAudioEffect("NormalMordel/Grid/GridSelect");
                selectGrid = grid;
                selectGrid.ShowGrid();
            }
            else if (grid == selectGrid) // 选择了同一个格子
            {
                PlayAudioEffect("NormalMordel/Grid/GridDeselect");
                grid.HideGrid();
                selectGrid = null;
            }
            else // 选择不同的格子
            {
                PlayAudioEffect("NormalMordel/Grid/GridSelect");
                selectGrid.HideGrid();
                selectGrid = grid;
                selectGrid.ShowGrid();
            }
        }
        else
        {
            PlayAudioEffect("NormalMordel/Grid/SelectFault");
            grid.HideGrid();
            grid.ShowCantBuild();
            if (selectGrid != null)
            {
                selectGrid.HideGrid();
            }
        }
    }

    #endregion

    #region 产生怪物的相关方法

    // 产怪的方法(产生一波)
    public void CreateMonster()
    {
        if (gameOver) return;
        creatingMonster = true;
        InvokeRepeating("InstantiateMonster", (float)1 / gameSpeed, (float)1 / gameSpeed);
    }

    // 产怪的方法(单个产怪)
    private void InstantiateMonster()
    {
        if (currentMonsterIndex == monstersID.Length)
        {
            StopCreateMonster();
            return;
        }

        PlayAudioEffect("NormalMordel/Monster/Create"); // 产生音效
        // 产生特效
        GameObject effect = GetItem("CreateMonsterEffect");
        effect.transform.SetParent(transform);
        effect.transform.position = mapMaker.monsterPointPosList[0];

        // 产生怪物
        if (currentMonsterIndex < monstersID.Length)
        {
            monsterBuilder.monsterID = level.rounds[level.currentRound].info.mMonsterIDList[currentMonsterIndex];
        }
        GameObject monster = monsterBuilder.GetProduct();
        monster.transform.SetParent(transform);
        monster.transform.position = mapMaker.monsterPointPosList[0];
        currentMonsterIndex++;
    }

    // 停止产怪的方法
    private void StopCreateMonster()
    {
        CancelInvoke();
    }

    // 增加回合数的方法
    private void AddRoundNum()
    {
        currentMonsterIndex = 0;
        killMonsterNum = 0;
        level.AddRoundIndex();
        level.HandleRound();
        // 更新有关UI
        normalModelPanel.topPage.UpdateRoundText();
    }

    #endregion

    #region 通过GameManager向工厂获取资源的系列方法(中介者)

    // 获取Sprite资源的方法
    public Sprite GetSprite(string resourcesPath)
    {
        return mGameManager.GetSprite(resourcesPath);
    }

    // 获取AudioClip资源的方法
    public AudioClip GetAudioClip(string resourcesPath)
    {
        return mGameManager.GetAudioClip(resourcesPath);
    }

    // 获取RuntimeAnimator资源的方法
    public RuntimeAnimatorController GetRuntimeAnimator(string resourcesPath)
    {
        return mGameManager.GetRuntimeAnimator(resourcesPath);
    }

    // 获取游戏物体的方法
    public GameObject GetItem(string itemName)
    {
        return mGameManager.GetItem(FactoryType.GameObject, itemName);
    }

    // 将游戏物体放回对象池的方法
    public void PushItem(string itemName, GameObject item)
    {
        mGameManager.PushItem(FactoryType.GameObject, itemName, item);
    }

    #endregion
}
