using System.Collections.Generic;

public class Level
{
    public int roundCount;
    public Round[] rounds;
    public int currentRound;

    public Level(int roundNum,List<Round.RoundInfo> infoList)
    {
        roundCount = roundNum;
        rounds = new Round[roundCount];
        // 初始化Round数组
        for(int i = 0;i < roundCount; i++)
        {
            rounds[i] = new Round(infoList[i].mMonsterIDList, i, this);
        }
        // 设置责任链
        for(int i = 0;i < roundCount; i++)
        {
            if (i == roundCount - 1) break;
            rounds[i].SetNextRound(rounds[i + 1]);
        }
        currentRound = 0;
    }

    // 当前波次的处理
    public void HandleRound()
    {
        if(currentRound == roundCount)
        {
            // TODO 胜利
            currentRound--;
            GameController.Instance.Win();
        }
        else if(currentRound == roundCount - 1) // 最后一波怪
        {
            // TODO UI及音效的处理
            GameController.Instance.ShowFinalWave();
            RoundLast();
        }
        else // 普通波次 
        {
            rounds[currentRound].Handle(currentRound);
        }
    }

    // 最后一波怪的Handle处理
    public void RoundLast()
    {
        rounds[currentRound].Handle(roundCount - 1);
    }

    // 进入下一个波次
    public void AddRoundIndex()
    {
        currentRound++;
    }
}
