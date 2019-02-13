using UnityEngine;
using System.Collections.Generic;
using System.IO;
using LitJson;

/// <summary>
/// 产生地图的类以及开发状态下地图编辑的工具类
/// </summary>
public class MapMaker : MonoBehaviour
{
#if Tool
    /* 开发状态下成员变量 */
    public bool drawLine; // 开发状态下开关,画辅助线开发设计
    public GameObject grid; // 开发使用的格子,不通过工厂加载
    public static MapMaker Instance { get; private set; } // 开发工具类的单例

#endif

    /* 地图的有关属性 */
    // 当前关卡的索引
    [HideInInspector] public int bigLevelID;
    [HideInInspector] public int levelID;

    // 地图分割行列数
    public const int row = 8;
    public const int col = 12;

    public GridPoint[,] gridPoints; // 全部的格子对象
    [HideInInspector] public Carrot carrot; // 萝卜对象

    // 怪物路径点列表及具体位置
    [HideInInspector] public List<GridPoint.GridIndex> monsterPointList = new List<GridPoint.GridIndex>();
    [HideInInspector] public List<Vector3> monsterPointPosList = new List<Vector3>();

    // 背景和道路的渲染
    private SpriteRenderer bgRenderer;
    private SpriteRenderer roadRenderer;

    public List<Round.RoundInfo> roundInfoList; // 怪物波次信息列表

    // 地图宽高
    private float mapWidth;
    private float mapHeight;
    // 格子宽高
    [HideInInspector] public float gridWidth;
    [HideInInspector] public float gridHeight;

    private void Awake()
    {
#if Tool 
        Instance = this;
        InitMapMaker();
#endif
    }

    // 初始化地图
    public void InitMapMaker()
    {
        CalcGridSize();
        gridPoints = new GridPoint[col, row];

        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
#if Tool
                GameObject item = Instantiate(grid);
#elif Game
                GameObject item = GameController.Instance.GetItem("Grid");
#endif
                item.transform.position = CorrectPos(new Vector3(gridWidth * x, gridHeight * y));
                item.transform.SetParent(transform);
                gridPoints[x, y] = item.GetComponent<GridPoint>();
                gridPoints[x, y].gridIndex = new GridPoint.GridIndex(x, y);
            }
        }

        bgRenderer = transform.Find("BG").GetComponent<SpriteRenderer>();
        roadRenderer = transform.Find("Road").GetComponent<SpriteRenderer>();
    }

    // 纠正位置的方法
    private Vector3 CorrectPos(Vector3 pos)
    {
        return new Vector2(pos.x - mapWidth / 2 + gridWidth / 2, pos.y - mapHeight / 2 + gridHeight / 2);
    }

    // 计算格子宽高
    private void CalcGridSize()
    {
        Vector2 leftDownPoint = Vector2.zero;
        Vector2 rightUpPoint = Vector2.one;

        Vector3 posOne = Camera.main.ViewportToWorldPoint(leftDownPoint);
        Vector3 posTwo = Camera.main.ViewportToWorldPoint(rightUpPoint);

        mapWidth = posTwo.x - posOne.x;
        mapHeight = posTwo.y - posOne.y;
        gridWidth = mapWidth / col;
        gridHeight = mapHeight / row;
    }

#if Tool
    // 格子划线辅助设计
    private void OnDrawGizmos()
    {
        if (drawLine)
        {
            CalcGridSize();
            Gizmos.color = Color.green;

            // 画行
            for (int y = 0; y <= row; y++)
            {
                Vector2 startPos = new Vector2(-mapWidth / 2, -mapHeight / 2 + gridHeight * y);
                Vector2 endPos = new Vector2(mapWidth / 2, -mapHeight / 2 + gridHeight * y);
                Gizmos.DrawLine(startPos, endPos);
            }
            // 画列
            for (int x = 0; x <= col; x++)
            {
                Vector2 startPos = new Vector2(-mapWidth / 2 + gridWidth * x, -mapHeight / 2);
                Vector2 endPos = new Vector2(-mapWidth / 2 + gridWidth * x, mapHeight / 2);
                Gizmos.DrawLine(startPos, endPos);
            }
        }
    }
    // 清除怪物路点的方法 
    public void ClearMonsterPoints()
    {
        monsterPointList.Clear();
    }

    // 恢复地图编辑的默认状态
    public void RecoverTowerPoint()
    {
        ClearMonsterPoints();
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                gridPoints[x, y].InitGridState();
            }
        }
    }

    // 初始化地图的方法
    public void InitMap()
    {
        bigLevelID = 0;
        levelID = 0;
        RecoverTowerPoint();
        roundInfoList.Clear();
    }

    // 生成LevelInfo对象来保存关卡信息
    private LevelInfo CreateLevelInfo()
    {
        LevelInfo info = new LevelInfo()
        {
            bigLevelID = bigLevelID,
            levelID = levelID,
        };
        info.gridPoints = new List<GridPoint.GridState>();
        for(int x = 0;x< col; x++)
        {
            for(int y =0;y < row; y++)
            {
                info.gridPoints.Add(gridPoints[x, y].gridState);
            }
        }
        info.monsterPath = new List<GridPoint.GridIndex>();
        for (int i = 0; i < monsterPointList.Count; i++)
        {
            info.monsterPath.Add(monsterPointList[i]); 
        }
        info.roundInfo = roundInfoList;
        Debug.Log("保存成功!");
        return info;
    }

    // 将制作的关卡信息保存为Json文件
    public void SaveLevelInfoByJson()
    {
        LevelInfo info = CreateLevelInfo();
        string filePath = Application.StreamingAssetsPath + "/Json/Level/Level" + bigLevelID + "_" + levelID + ".json";
        string jsonStr = JsonMapper.ToJson(info);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(jsonStr);
        sw.Close();
    }
#endif
#if Game

    // 加载地图的方法
    public void  Loadmap(int bigLevel,int level)
    {
        LoadLevelInfo(LoadJsonToLevelInfo("Level" + bigLevel + "_" + level + ".json"));
        //得到怪物路径点的具体位置列表
        monsterPointPosList = new List<Vector3>();
        for(int i = 0;i < monsterPointList.Count; i++)
        {
            monsterPointPosList.Add(gridPoints[monsterPointList[i].xIndex, monsterPointList[i].yIndex].transform.position);
        }
        // 起始点与终止点的赋值
        GameObject startPoint = GameController.Instance.GetItem("StartPoint");
        startPoint.transform.position = monsterPointPosList[0];
        startPoint.transform.SetParent(transform);
        GameObject endPoint = GameController.Instance.GetItem("Carrot");
        endPoint.transform.position = monsterPointPosList[monsterPointPosList.Count-1] + new Vector3(0,0.5f,0);
        endPoint.transform.SetParent(transform);
        carrot = endPoint.GetComponent<Carrot>();
    }

#endif

    // 读取Json转化为LevelInfo对象
    public LevelInfo LoadJsonToLevelInfo(string fileName)
    {
        LevelInfo info = new LevelInfo();
        string filePath = Application.streamingAssetsPath + "/Json/Level/" + fileName;
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            info = JsonMapper.ToObject<LevelInfo>(jsonStr);
            return info;
        }
        Debug.LogError("Json文件加载异常:加载失败路径为:" + filePath);
        return null;
    }

    // 读取LevelInfo的数据
    public void LoadLevelInfo(LevelInfo info)
    {
        bigLevelID = info.bigLevelID;
        levelID = info.levelID;
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                gridPoints[x, y].gridState = info.gridPoints[y + x * row];
                // 更新格子的状态
                gridPoints[x, y].UpdateGrid();
            }
        }
        monsterPointList.Clear();
        roundInfoList = new List<Round.RoundInfo>();
        monsterPointList = info.monsterPath;
        roundInfoList = info.roundInfo;

        bgRenderer.sprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + bigLevelID + "/BG" + levelID / 3);
        roadRenderer.sprite = Resources.Load<Sprite>("Pictures/NormalMordel/Game/" + bigLevelID + "/Road" + levelID);

    }
}
