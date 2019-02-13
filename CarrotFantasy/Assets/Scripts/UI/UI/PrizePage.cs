using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrizePage : MonoBehaviour
{
    private Image prizeImg;
    private Image instructionImg;
    private Text prizeNameTxt;
    private Animator animator;
    private NormalModelPanel normalModelPanel;

    private void Awake()
    {
        prizeImg = transform.Find("Img_Prize").GetComponent<Image>();
        instructionImg = transform.Find("Img_Instruction").GetComponent<Image>();
        prizeNameTxt = transform.Find("Img_Prize/Txt_PrizeName").GetComponent<Text>();
        animator = GetComponent<Animator>();
        normalModelPanel = GetComponentInParent<NormalModelPanel>();
    }

    private void OnEnable()
    {
        int randomNum = Random.Range(0, 10);
        string prizeName = "";
        List<MonsterPetData> monsterPetDataList = normalModelPanel.GetMonsterPetData();
        if (randomNum == 0 && monsterPetDataList.Count < 3) // 宠物蛋 0.01概率
        {
            int eggID;
            do
            {
                eggID = Random.Range(1, 4);
            } while (HasThePet(monsterPetDataList, eggID));

            // 生成宠物蛋的信息
            MonsterPetData monsterPetData = new MonsterPetData
            {
                monsterID = eggID,
                monsterLevel = 1,
                remainCookies = 0,
                remainMilks = 0
            };
            prizeName = "宠物蛋";
            normalModelPanel.SetMonsterEgg(monsterPetData);
        }
        else if (randomNum == 1 || randomNum == 0) // 怪物窝 0.02概率
        {
            prizeName = "窝";
            normalModelPanel.SetMonsterData(0, 0, 1);
        }
        else
        {
            if (randomNum <= 5) // 饼干
            {
                prizeName = "饼干";
                normalModelPanel.SetMonsterData(10, 0, 0);
            }
            else // 牛奶
            {
                prizeName = "牛奶";
                normalModelPanel.SetMonsterData(0, 50, 0);
            }
        }

        // 更新页面的UI显示
        prizeNameTxt.text = prizeName;
        int prizeID = GetPrizeID(prizeName);
        prizeImg.sprite = normalModelPanel.gameController.GetSprite("MonsterNest/Prize/Prize" + prizeID);
        instructionImg.sprite = normalModelPanel.gameController.GetSprite("MonsterNest/Prize/Instruction" + prizeID);
        animator.Play("PrizePage");
    }

    private bool HasThePet(List<MonsterPetData> monsterPetDataList, int monsterID)
    {
        for (int i = 0; i < monsterPetDataList.Count; i++)
        {
            if (monsterPetDataList[i].monsterID == monsterID)
            {
                return true;
            }
        }
        return false;
    }

    private int GetPrizeID(string prizeName)
    {
        switch (prizeName)
        {
            case "牛奶":
                return 1;
            case "饼干":
                return 2;
            case "窝":
                return 3;
            default:
                return 4;
        }
    }

    public void OnKnowButtonClick()
    {
        normalModelPanel.PlayButtonAudioEffect();
        normalModelPanel.HidePrize();
    }
}
