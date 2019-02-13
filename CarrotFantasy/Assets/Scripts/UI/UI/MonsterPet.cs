using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 宠物信息
/// </summary>
public class MonsterPet : MonoBehaviour
{
    [HideInInspector] public MonsterNestPanel monsterNestPanel;
    public MonsterPetData monsterPetData;
    public Sprite[] buttonSprites; //01 23 45 牛奶 饼干(可用 不可用)

    private GameObject[] monsterLevelGo;//宠物对应的三个等级的游戏物体

    // Egg
    private GameObject instructionGo; // 提示没有怪物窝的提示框

    // Baby
    private GameObject feedGo;
    private Text milkTxt;
    private Text cookiesTxt;
    private Button milkButton;
    private Button cookiesButton;
    private Image milkImg;
    private Image cookieImg;

    // Normal
    private GameObject leftTalkGo;
    private GameObject rightTalkGo;

    private void Awake()
    {
        monsterLevelGo = new GameObject[3];
        monsterLevelGo[0] = transform.Find("Emp_Egg").gameObject;
        monsterLevelGo[1] = transform.Find("Emp_Baby").gameObject;
        monsterLevelGo[2] = transform.Find("Emp_Normal").gameObject;

        //Egg
        instructionGo = monsterLevelGo[0].transform.Find("Img_Instruction").gameObject;
        instructionGo.SetActive(false);

        //Baby
        feedGo = monsterLevelGo[1].transform.Find("Emp_Feed").gameObject;
        feedGo.SetActive(false);
        milkTxt = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Milk/Txt_Milk").GetComponent<Text>();
        cookiesTxt = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Cookie/Txt_Milk").GetComponent<Text>();
        milkButton = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Milk").GetComponent<Button>();
        cookiesButton = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Cookie").GetComponent<Button>();
        milkImg = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Milk").GetComponent<Image>();
        cookieImg = monsterLevelGo[1].transform.Find("Emp_Feed/Btn_Cookie").GetComponent<Image>();

        //Normal
        leftTalkGo = transform.Find("Img_TalkLeft").gameObject;
        rightTalkGo = transform.Find("Img_TalkRight").gameObject;
    }

    private void Start()
    {
        InitMonsterPet();
    }

    //初始化宠物
    public void InitMonsterPet()
    {
        if (monsterPetData.remainMilks == 0)
        {
            monsterPetData.remainMilks = monsterPetData.monsterID * 150;
        }
        if (monsterPetData.remainCookies == 0)
        {
            monsterPetData.remainCookies = monsterPetData.monsterID * 30;
        }
        ShowMonster();
    }

    //点击宠物需要触发的事件方法
    public void ClickPet()
    {
        GameManager.Instance.AudioManager.PlayEffectMusic(GameManager.Instance.GetAudioClip("MonsterNest/PetSound" + monsterPetData.monsterLevel));
        switch (monsterPetData.monsterLevel)
        {
            case 1:
                if (GameManager.Instance.PlayerManager.NestCount >= 1)
                {
                    GameManager.Instance.PlayerManager.NestCount--;
                    //升级 更新显示
                    ToNormal();
                    monsterPetData.monsterLevel++;
                    ShowMonster();
                    monsterNestPanel.UpdateText();
                }
                else
                {
                    instructionGo.SetActive(true);
                    Invoke("CloseTalkUI", 2);
                }

                break;
            case 2:
                if (feedGo.activeSelf)
                {
                    feedGo.SetActive(false);
                }
                else
                {
                    feedGo.SetActive(true);
                    // 按钮图片的显示
                    if (GameManager.Instance.PlayerManager.MilkCount == 0)
                    {
                        milkImg.sprite = buttonSprites[1];
                        milkButton.interactable = false;
                    }
                    else
                    {
                        milkImg.sprite = buttonSprites[0];
                        milkButton.interactable = true;
                    }
                    if (GameManager.Instance.PlayerManager.CookiesCount == 0)
                    {
                        cookieImg.sprite = buttonSprites[3];
                        cookiesButton.interactable = false;
                    }
                    else
                    {
                        cookieImg.sprite = buttonSprites[2];
                        cookiesButton.interactable = true;
                    }
                    // 显示需不需要喂养牛奶饼干
                    if (monsterPetData.remainMilks == 0)
                    {
                        milkButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        milkTxt.text = monsterPetData.remainMilks.ToString();
                        milkButton.gameObject.SetActive(true);
                    }
                    if (monsterPetData.remainCookies == 0)
                    {
                        cookiesButton.gameObject.SetActive(false);
                    }
                    else
                    {
                        cookiesTxt.text = monsterPetData.remainCookies.ToString();
                        cookiesButton.gameObject.SetActive(true);
                    }
                }
                break;
            case 3:
                int randomNum = Random.Range(0, 2);
                if (randomNum == 1)
                {
                    rightTalkGo.SetActive(true);
                    Invoke("CloseTalkUI", 2);
                }
                else
                {
                    leftTalkGo.SetActive(true);
                    Invoke("CloseTalkUI", 2);
                }
                break;
            default:
                break;
        }

    }

    //关闭对话框
    private void CloseTalkUI()
    {
        instructionGo.SetActive(false);
        rightTalkGo.SetActive(false);
        leftTalkGo.SetActive(false);
    }

    //成长方法
    private void ToNormal()
    {
        GameManager.Instance.AudioManager.PlayEffectMusic(GameManager.Instance.GetAudioClip("MonsterNest/PetChange"));
        if (monsterPetData.remainMilks == 0 && monsterPetData.remainCookies == 0)
        {
            monsterPetData.monsterLevel++;
            if (monsterPetData.monsterLevel == 3)
            {
                GameManager.Instance.PlayerManager.LevelStageList[monsterPetData.monsterID * 5 - 1].unLocked = true;
                GameManager.Instance.PlayerManager.UnLockedLevelNum[monsterPetData.monsterLevel - 1] += 1;
                GameManager.Instance.PlayerManager.HideLevelNum++;
                ShowMonster();
            }
            else
            {
                ShowMonster();
            }
        }
        SaveMonsterData();
    }

    //显示当前等级的宠物
    private void ShowMonster()
    {
        for (int i = 0; i < monsterLevelGo.Length; i++)
        {
            monsterLevelGo[i].SetActive(false);
            if ((i + 1) == monsterPetData.monsterLevel)
            {
                monsterLevelGo[i].SetActive(true);
                Sprite petSprite = null;
                switch (monsterPetData.monsterLevel)
                {
                    case 1:
                        petSprite = GameManager.Instance.GetSprite("MonsterNest/Monster/Egg/" + monsterPetData.monsterID.ToString());
                        break;
                    case 2:
                        petSprite = GameManager.Instance.GetSprite("MonsterNest/Monster/Baby/" + monsterPetData.monsterID.ToString());
                        break;
                    case 3:
                        petSprite = GameManager.Instance.GetSprite("MonsterNest/Monster/Normal/" + monsterPetData.monsterID.ToString());
                        break;
                    default:
                        break;
                }
                Image monsterImage = monsterLevelGo[i].transform.Find("Img_Pet").GetComponent<Image>();
                monsterImage.sprite = petSprite;
                monsterImage.SetNativeSize();
                float imageScale = 0;
                if (monsterPetData.monsterLevel == 1)
                {
                    imageScale = 2;
                }
                else
                {
                    imageScale = 1 + (monsterPetData.monsterLevel - 1) * 0.5f;
                }

                monsterImage.transform.localScale = new Vector3(imageScale, imageScale, 1);
            }
        }
    }

    //保存宠物数据
    private void SaveMonsterData()
    {
        for (int i = 0; i < GameManager.Instance.PlayerManager.MonsterPetDataList.Count; i++)
        {
            if (GameManager.Instance.PlayerManager.MonsterPetDataList[i].monsterID == monsterPetData.monsterID)
            {
                GameManager.Instance.PlayerManager.MonsterPetDataList[i] = monsterPetData;
            }
        }
    }

    //喂牛奶
    public void FeedMilk()
    {
        GameManager.Instance.AudioManager.PlayEffectMusic(GameManager.Instance.GetAudioClip("MonsterNest/Feed01"));
       GameObject heartGo = GameManager.Instance.GetItem(FactoryType.UI,"Img_Heart");
        heartGo.transform.position = transform.position;
        monsterNestPanel.SetCanvasTrans(heartGo.transform);
        if (GameManager.Instance.PlayerManager.MilkCount >= monsterPetData.remainMilks)
        {
            GameManager.Instance.PlayerManager.MilkCount -= monsterPetData.remainMilks;
            monsterPetData.remainMilks = 0;          
        }
        else
        {
            monsterPetData.remainMilks -= GameManager.Instance.PlayerManager.MilkCount;
            GameManager.Instance.PlayerManager.MilkCount = 0;
            milkButton.gameObject.SetActive(false);
        }
        //更新文本
        monsterNestPanel.UpdateText();
        feedGo.SetActive(false);
        Invoke("ToNormal", 0.433f);
    }

    //喂饼干
    public void FeedCookie()
    {
        GameManager.Instance.AudioManager.PlayEffectMusic(GameManager.Instance.GetAudioClip("MonsterNest/Feed02"));
        GameObject heartGo = GameManager.Instance.GetItem(FactoryType.UI, "Img_Heart");
        heartGo.transform.position = transform.position;
        monsterNestPanel.SetCanvasTrans(heartGo.transform);
        if (GameManager.Instance.PlayerManager.CookiesCount >= monsterPetData.remainCookies)
        {

            GameManager.Instance.PlayerManager.CookiesCount -= monsterPetData.remainCookies;
            monsterPetData.remainCookies = 0;          
        }
        else
        {
            monsterPetData.remainCookies -= GameManager.Instance.PlayerManager.CookiesCount;
            GameManager.Instance.PlayerManager.CookiesCount = 0;
            cookiesButton.gameObject.SetActive(false);
        }
        //更新文本
        monsterNestPanel.UpdateText();
        feedGo.SetActive(false);
        Invoke("ToNormal", 0.433f);
    }
}

/// <summary>
/// 怪物窝宠物信息
/// </summary>
public struct MonsterPetData
{
    public int monsterLevel;
    public int remainCookies;
    public int remainMilks;
    public int monsterID;
}
