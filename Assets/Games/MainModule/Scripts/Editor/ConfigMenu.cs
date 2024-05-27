using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ConfigMenu
{
    [MenuItem("Assets/Create/Configs/LevelsConfig")]
    static void CreateLevelConfig()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);//获取当前用户选中的资源路径
        if(!AssetDatabase.IsValidFolder(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }
        
        LevelsConfig levConfig = ScriptableObject.CreateInstance<LevelsConfig>();//将配置类型传递，创建一个关卡配置的对象

        AssetDatabase.CreateAsset(levConfig,string.Format("{0}/LevelsConfig.asset",path));
    }

    [MenuItem("Assets/Create/Configs/EnemiesConfig")]
    static void CreateEnemiesConfig()//敌人配表
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (!AssetDatabase.IsValidFolder(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }

        EnemiesConfig config = ScriptableObject.CreateInstance<EnemiesConfig>();
        AssetDatabase.CreateAsset(config, string.Format("{0}/EnemiesConfig.asset", path));
    }

    [MenuItem("Assets/Create/Configs/TowersConfig")]
    static void CreateTowersConfig()//炮塔陪标
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (!AssetDatabase.IsValidFolder(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }

        TowersConfig config = ScriptableObject.CreateInstance<TowersConfig>();
        AssetDatabase.CreateAsset(config, string.Format("{0}/TowersConfig.asset", path));
    }

}


