using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 玩家的管理,负责保存以及加载游戏的信息
/// </summary>
public class PlayerManager
{
    // 统计数据
    public int NormalNum { get; set; } // 通过的冒险模式总数
    public int HideLevelNum { get; set; } // 解锁的隐藏关卡数目
    public int BossNum { get; set; } // 通过的Boss关卡数目
    public int MoneyNum { get; set; } // 获得总金币数
    public int ClearMonsterCount { get; set; } // 清理怪物总数
    public int ClearBossCount { get; set; } // 清理Boss总数
    public int ClearItemCount { get; set; } // 清理道具总数

    // 怪物窝拥有道具数据
    public int CookiesCount { get; set; }
    public int MilkCount { get; set; }
    public int NestCount { get; set; }
    public int Diamands { get; private set; }
    public List<MonsterPetData> MonsterPetDataList { get; set; }

    // 游戏数据
    public List<bool> UnLockedBigLevelList { get; set; } // 解锁的大关卡列表
    public List<Stage> LevelStageList { get; set; } // 小关卡信息列表
    public List<int> UnLockedLevelNum { get; set; } // 大关卡下解锁的小关卡数目

    ////开发 用于数据初始化Json制作
    //public PlayerManager()
    //{
    //    NormalNum = 0;
    //    HideLevelNum = 0;
    //    BossNum = 0;
    //    MoneyNum = 0;
    //    ClearBossCount = 0;
    //    ClearItemCount = 0;
    //    ClearMonsterCount = 0;
    //    CookiesCount = 0;
    //    MilkCount = 0;
    //    NestCount = 0;
    //    Diamands = 0;

    //    MonsterPetDataList = new List<MonsterPetData>();
    //    UnLockedLevelNum = new List<int>() { 1, 0, 0 };
    //    UnLockedBigLevelList = new List<bool> { true, false, false };
    //    LevelStageList = new List<Stage>()
    //    {
    //        new Stage(new int[]{1},1,false,0,1,1,true,false,10),
    //        new Stage(new int[]{2},1,false,0,2,1,false,false,10),
    //        new Stage(new int[]{1,2},2,false,0,3,1,false,false,10),
    //        new Stage(new int[]{3},1,false,0,4,1,false,false,10),
    //        new Stage(new int[]{1,2,3},3,false,0,5,1,false,true,10),

    //        new Stage(new int[]{2,3},2,false,0,1,2,false,false,10),
    //        new Stage(new int[]{1,3},2,false,0,2,2,false,false,10),
    //        new Stage(new int[]{4},1,false,0,3,2,false,false,10),
    //        new Stage(new int[]{1,4},2,false,0,4,2,false,false,10),
    //        new Stage(new int[]{2,4},2,false,0,5,2,false,true,10),

    //        new Stage(new int[]{3,4},2,false,0,1,3,false,false,10),
    //        new Stage(new int[]{5},1,false,0,2,3,false,false,10),
    //        new Stage(new int[]{4,5},2,false,0,3,3,false,false,10),
    //        new Stage(new int[]{1,3,5},3,false,0,4,3,false,false,10),
    //        new Stage(new int[]{1,4,5},3,false,0,5,3,false,true,10)
    //    };
    //}

    public void SaveData()
    {
        Memento memento = new Memento();
        memento.Save();
    }

    public void LoadData()
    {
        Memento memento = new Memento();
        PlayerManager playerManager = memento.Load();
        SetValue(playerManager);
    }

    // 通过反射给PlayerManager赋值(赋值不赋地址)
    private void SetValue(PlayerManager playerManager)
    {
        Type type = GetType();
        var pList = type.GetProperties().ToList();
        for (int i = 0; i < pList.Count; i++)
        {
            PropertyInfo gc = type.GetProperty(pList[i].Name);
            gc.SetValue(this, pList[i].GetValue(playerManager, null), null);
        }
    }

    public void BuyMonsterItem(int price)
    {
        Diamands -= price;
    }
}
