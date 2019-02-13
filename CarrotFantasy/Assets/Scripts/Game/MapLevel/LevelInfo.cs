using System.Collections.Generic;

/// <summary>
/// 关卡信息数据(用于Json对象转换)
/// </summary>
public class LevelInfo
{
    public int bigLevelID;
    public int levelID;

    public List<GridPoint.GridState> gridPoints;
    public List<GridPoint.GridIndex> monsterPath;
    public List<Round.RoundInfo> roundInfo;
}
