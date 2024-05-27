using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using XFABManager;

public class LevelEnemyEditor 
{


    private static string levels_config_path = "Assets/Games/MainModule/Configs/LevelsConfig.asset";

    private static LevelsConfig _levelsConfig;

    private static LevelsConfig levelsConfig
    {
        get {

            if (_levelsConfig == null)
                _levelsConfig = AssetDatabase.LoadAssetAtPath<LevelsConfig>(levels_config_path);

            return _levelsConfig;
        }
    }


    private static string enemies_config_path = "Assets/Games/MainModule/Configs/EnemiesConfig.asset";

    private static EnemiesConfig _enemiesConfig;//获取敌人配表
    private static EnemiesConfig enemiesConfig
    {
        get {
            if (_enemiesConfig == null)
            {
                _enemiesConfig = AssetDatabase.LoadAssetAtPath<EnemiesConfig>(enemies_config_path);//加载关卡配表中的路径
            }

            return _enemiesConfig;
        }
    }

    [MenuItem("Tool/LevelEnemyConfig/Read")]
    static void LevelConfigRead() {
        GameObject obj = Selection.activeGameObject;
        if (!obj.name.StartsWith("level:"))
        {
            return;
        }

        int levelId = int.Parse(obj.name.Split(":")[1]);

        LevelInfo info = levelsConfig.GetLevelInfo(levelId);

        for (int i = obj.transform.childCount - 1; i >=0; i--)
        {
            GameObject childObj = obj.transform.GetChild(i).gameObject;
            GameObject.DestroyImmediate(childObj);
        }

        for (int i = 0; i < info.enemyWaves.Count; i++)
        {
            EnemyWaves wave = info.enemyWaves[i];

            GameObject waveObj = new GameObject(string.Format("wave{0}:{1}", i, wave.fight_buttton));
            waveObj.transform.SetParent(obj.transform);
            waveObj.transform.localPosition = new Vector3(0, -i * 10, 0);

            for (int j = 0; j < wave.enemies.Count; j++) {
                EnemyData enemyData = wave.enemies[j];
                Transform path = waveObj.transform.Find(enemyData.path);
                if (path == null) { 
                    GameObject pathObj = new GameObject(enemyData.path);
                    pathObj.transform.SetParent(waveObj.transform);

                    int pathNumber = int.Parse( enemyData.path.Replace("path",string.Empty));

                    pathObj.transform.localPosition = new Vector3(0,-pathNumber,0);
                    path = pathObj.transform;
                }

                EnemyInfo enemyInfo = enemiesConfig.GetEnemyInfo(enemyData.enemyId);
                GameObject prefab = AssetBundleManager.LoadAsset<GameObject>("MainModule", enemyInfo.prefab_name);

                GameObject enemyObj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                enemyObj.name = prefab.name;
                enemyObj.transform.SetParent(path);
                enemyObj.transform.localPosition = new Vector3(-enemyData.time, 0, 0);
            }

        }


    }

    
    
    [MenuItem("Tool/LevelEnemyConfig/Write")]
    static void LevelConfigWrite() {

        GameObject obj = Selection.activeGameObject;
        if (!obj.name.StartsWith("level:"))
        {
            return;
        }

        int levelId = int.Parse(obj.name.Split(":")[1]);


        List<EnemyWaves> enemyWaves = new List<EnemyWaves>();

        for (int i = 0; i < obj.transform.childCount; i++) {//遍历所有游戏物体子节点
            Transform child = obj.transform.GetChild(i);
            if (!child.name.StartsWith("wave"))
            {
                continue;
            }

            EnemyWaves wave = new EnemyWaves();
            string fight_button_name = child.name.Split(":")[1];
            wave.fight_buttton = fight_button_name;

            for (int j = 0; j < child.transform.childCount; j++) { 
                Transform path_child = child.transform.GetChild(j);

                for (int k = 0; k < path_child.childCount; k++)//继续解析子节点获取路径
                {
                    // 具体的敌人
                    Transform enemy = path_child.GetChild(k);

                    EnemyData data = new EnemyData();
                    data.time = Mathf.Abs( enemy.transform.localPosition.x);
                    data.enemyId = GetEnemyIdByPrefabName(enemy.transform.name);
                    data.path = path_child.name;

                    wave.enemies.Add(data);
                }

            }

            enemyWaves.Add(wave); 
        }

        Debug.Log(levelsConfig == null);

        LevelInfo info = levelsConfig.GetLevelInfo(levelId);
        info.enemyWaves = enemyWaves;

        EditorUtility.SetDirty(levelsConfig);//保存配表

        Debug.Log("保存成功!");
    }


    static int GetEnemyIdByPrefabName(string name) {

        if(name.Contains(" "))
        {
            name = name.Split(" ")[0];//避免复制预制体时空格产生影响
        }
            

        foreach (var item in enemiesConfig.enemies)
        {
            if (item.prefab_name.Equals(name))
            {
                return item.id;
            }
        }

        return 0;
    }

}
