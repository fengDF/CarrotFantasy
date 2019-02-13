
/// <summary>
/// 怪物波次信息
/// </summary>
public class Round
{
    [System.Serializable]
    public struct RoundInfo
    {
        public int[] mMonsterIDList;
        public int monsterNum;
    }

    public RoundInfo info;
    private Round nextRound;
    private readonly int roundID;
    private readonly Level level;

    public Round(int[] monstersID,int roundID,Level level)
    {
        info.mMonsterIDList = monstersID;
        this.roundID = roundID;
        this.level = level;
    }

    public void SetNextRound(Round round)
    {
        nextRound = round;
    }

    public void Handle(int roundID)
    {
        if(this.roundID < roundID)
        {
            nextRound.Handle(roundID);
        }
        else
        {
            // 产生怪物
            GameController.Instance.monstersID = info.mMonsterIDList;
            GameController.Instance.CreateMonster();
        }
    }
}
