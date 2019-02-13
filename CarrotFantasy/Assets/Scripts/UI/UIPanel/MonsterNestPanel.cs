using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 怪物窝面板
/// </summary>
public class MonsterNestPanel : BasePanel
{
    private GameObject shopGO;
    private List<GameObject> monsterPetGOList;
    private Transform monstersGroupTrans;

    private Text cookiesTxt;
    private Text milkTxt;
    private Text nestTxt;
    private Text diamands;

    protected override void Awake()
    {
        base.Awake();

        shopGO = transform.Find("ShopPage").gameObject;
        monstersGroupTrans = transform.Find("Img_Nest/Emp_MonsterGroup");
        cookiesTxt = transform.Find("TopPage/Txt_CookiesCount").GetComponent<Text>();
        milkTxt = transform.Find("TopPage/Txt_MilkCount").GetComponent<Text>();
        nestTxt = transform.Find("TopPage/Txt_NestCount").GetComponent<Text>();
        diamands = shopGO.transform.Find("Img_BG/Img_Diamands/Txt_Diamands").GetComponent<Text>();
        monsterPetGOList = new List<GameObject>();
        // 预先加载图片资源
        for (int i = 1; i < 4; i++)
        {
            mUIFacade.GetSprite("MonsterNest/Monster/Egg/" + i);
            mUIFacade.GetSprite("MonsterNest/Monster/Baby/" + i);
            mUIFacade.GetSprite("MonsterNest/Monster/Normal/" + i);
        }
    }

    public override void InitPanel()
    {
        for (int i = 0; i < monsterPetGOList.Count; i++)
        {
            mUIFacade.PushItem(FactoryType.UI, "Emp_Monsters", monsterPetGOList[i]);
        }
        monsterPetGOList.Clear();
        List<MonsterPetData> monsterPetDataList = mUIFacade.GetMonsterPetData();

        for (int i = 0; i < monsterPetDataList.Count; i++)
        {
            if (monsterPetDataList[i].monsterID != 0)
            {
                GameObject monsterPetGo = mUIFacade.GetItem(FactoryType.UI, "Emp_Monsters");
                monsterPetGo.GetComponent<MonsterPet>().monsterPetData = monsterPetDataList[i];
                monsterPetGo.GetComponent<MonsterPet>().monsterNestPanel = this;
                monsterPetGo.GetComponent<MonsterPet>().InitMonsterPet();
                monsterPetGo.transform.SetParent(monstersGroupTrans);
                monsterPetGo.transform.localScale = Vector3.one;
                monsterPetGOList.Add(monsterPetGo);
            }
        }
        UpdateText();
    }

    //更新文本
    public void UpdateText()
    {
        int[] data = mUIFacade.GetMonsterPetItem();
        cookiesTxt.text = data[0].ToString();
        milkTxt.text = data[1].ToString();
        nestTxt.text = data[2].ToString();
        diamands.text = data[3].ToString();
    }

    // 返回主场景的按钮
    public void OnHomeButtonClcik()
    {
        mUIFacade.PlayButtonAudioEffect();
        mUIFacade.ChangeSceneState(new MainSceneState(mUIFacade));
    }

    public void SetCanvasTrans(Transform uITrans)
    {
        uITrans.SetParent(mUIFacade.canvas);
    }

    #region 商店有关的方法

    public void ShowShop()
    {
        mUIFacade.PlayButtonAudioEffect();
        shopGO.SetActive(true);
    }

    public void CloseShop()
    {
        mUIFacade.PlayButtonAudioEffect();
        shopGO.SetActive(false);
    }

    public void BuyNest()
    {
        if (mUIFacade.GetMonsterPetItem()[3] >= 60)
        {
            mUIFacade.PlayButtonAudioEffect();
            mUIFacade.BuyMonsterItem(60);
            mUIFacade.SetMonsterData(0, 0, 1);
            UpdateText();
        }
    }

    public void BuyMilk()
    {
        if (mUIFacade.GetMonsterPetItem()[3] >= 1)
        {
            mUIFacade.PlayButtonAudioEffect();
            mUIFacade.BuyMonsterItem(1);
            mUIFacade.SetMonsterData(0, 10, 0);
            UpdateText();
        }
    }

    public void BuyCookie()
    {
        if (mUIFacade.GetMonsterPetItem()[3] >= 10)
        {
            mUIFacade.PlayButtonAudioEffect();
            mUIFacade.BuyMonsterItem(10);
            mUIFacade.SetMonsterData(15, 0, 0);
            UpdateText();
        }
    }

    #endregion
}
