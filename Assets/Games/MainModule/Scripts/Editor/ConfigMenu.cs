using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class ConfigMenu
{
    [MenuItem("Assets/Create/Configs/LevelsConfig")]
    static void CreateLevelConfig()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);//��ȡ��ǰ�û�ѡ�е���Դ·��
        if(!AssetDatabase.IsValidFolder(path))
        {
            path = System.IO.Path.GetDirectoryName(path);
        }
        
        LevelsConfig levConfig = ScriptableObject.CreateInstance<LevelsConfig>();//���������ʹ��ݣ�����һ���ؿ����õĶ���

        AssetDatabase.CreateAsset(levConfig,string.Format("{0}/LevelsConfig.asset",path));
    }

    [MenuItem("Assets/Create/Configs/EnemiesConfig")]
    static void CreateEnemiesConfig()//�������
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
    static void CreateTowersConfig()//�������
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


