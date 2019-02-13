using LitJson;
using System.IO;
using UnityEngine;

/// <summary>
/// 玩家数据信息读取保存(备忘录)
/// </summary>
public class Memento
{
    public void Save()
    {
        PlayerManager playerManager = GameManager.Instance.PlayerManager;
        string filePath = Application.streamingAssetsPath + "/Json/" + "playerManager.json";
        string saveJsonStr = JsonMapper.ToJson(playerManager);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
    }

    public PlayerManager Load()
    {
        PlayerManager playerManager = new PlayerManager();
        string filePath = "";
        if (GameManager.Instance.initPlayerManager)
        {
            filePath = Application.streamingAssetsPath + "/Json/" + "playerManager_init.json";
        }
        else
        {
            filePath = Application.streamingAssetsPath + "/Json/" + "playerManager.json";
        }

        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string data = sr.ReadToEnd();
            sr.Close();
            playerManager = JsonMapper.ToObject<PlayerManager>(data);
            return playerManager;
        }
        else
        {
            Debug.LogError("PlayerManager读取失败!,失败路径: " + filePath);
        }
        return null;
    }
}
