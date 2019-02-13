
/// <summary>
/// 游戏关卡信息类
/// </summary>
public class Stage
{
    public int[] towerIDList; // 可以使用的塔列表
    public int towerListLen; // 塔列表长度
    public bool allClear; // 是否全部清除道具
    public int carrotState; // 萝卜的状态 (0:未通关 1:金萝卜 2:银萝卜 3:普通萝卜)
    public int levelID;// 关卡ID
    public int bigLevelID;// 大关卡ID
    public bool unLocked; // 是否未解锁
    public bool isRewardLevel; // 是否奖励关卡
    public int totalWave; // 怪物总波数

    // 用于开发测试的构造方法
    //public Stage(int[] towerIDList, int towerListLen,bool allClear,int carrotState,int levelID,int bigLevelID,
    //    bool unLocked,bool isRewardLevel,int totalWave)
    //{
    //    this.towerIDList = towerIDList;
    //    this.towerListLen = towerListLen;
    //    this.allClear = allClear;
    //    this.carrotState = carrotState;
    //    this.levelID = levelID;
    //    this.bigLevelID = bigLevelID;
    //    this.unLocked = unLocked;
    //    this.isRewardLevel = isRewardLevel;
    //    this.totalWave = totalWave;
    //}
}
