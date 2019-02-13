using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 地图编辑的工具类,辅助开发设计关卡地图
/// </summary>
#if Tool
[CustomEditor(typeof(MapMaker))]
public class MapTool : Editor
{
    private MapMaker mapMaker;

    private List<FileInfo> fileList = new List<FileInfo>(); // 关卡文件的列表
    private string[] fileNames;

    private int selectIndex; // 起始编号

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying) // 在程序运行时才进行地图编辑的绘制
        {
            mapMaker = MapMaker.Instance;

            EditorGUILayout.BeginHorizontal(); // 第一行内容的绘制
            fileNames = GetFileNames(fileList); // 获取操作的文件名
            int currentIndex = EditorGUILayout.Popup(selectIndex, fileNames);
            if (currentIndex != selectIndex) // 当前选择是否发生改变
            {
                selectIndex = currentIndex;
                // 初始化地图的方法
                mapMaker.InitMap();
                // 加载当前选择的Level文件
                mapMaker.LoadLevelInfo(mapMaker.LoadJsonToLevelInfo(fileNames[selectIndex]));
            }
            if (GUILayout.Button("读取关卡文件列表"))
            {
                // 读取关卡列表的方法
                LoadLevelFiles();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(); // 第二行内容的绘制(两个重置设置的按钮)
            if (GUILayout.Button("恢复地图编辑器默认状态"))
            {
                // 恢复编辑器默认状态
                mapMaker.RecoverTowerPoint();
            }

            if (GUILayout.Button("清除怪物路点"))
            {
                // 清除怪物路点
                mapMaker.ClearMonsterPoints();
            }
            EditorGUILayout.EndHorizontal();

            // 最后保存设置的按钮
            if (GUILayout.Button("保存当前关卡设置"))
            {
                // 保存当前关卡设置
                mapMaker.SaveLevelInfoByJson();
            }

        }
    }

    // 加载关卡数据文件
    private void LoadLevelFiles()
    {
        ClearFileList();
        fileList = GetLevelFiles();
    }

    // 清除文件列表
    private void ClearFileList()
    {
        fileList.Clear();
        selectIndex = -1;
    }

    // 读取关卡列表
    private List<FileInfo> GetLevelFiles()
    {
        string[] files = Directory.GetFiles(Application.dataPath + "/Resources/Json/Level/", "*.json");

        List<FileInfo> list = new List<FileInfo>();
        for (int i = 0; i < files.Length; i++)
        {
            list.Add(new FileInfo(files[i]));
        }

        return list;
    }

    // 获取关卡文件的名字
    private string[] GetFileNames(List<FileInfo> fileList)
    {
        List<string> names = new List<string>();
        foreach (var file in fileList)
        {
            names.Add(file.Name);
        }

        return names.ToArray();
    }
}
#endif