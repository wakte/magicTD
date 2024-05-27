using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;



[Serializable]
public class EnemyData 
{
    public int enemyId;
    public float time;
    public string path;
}


[Serializable]
public class EnemyWaves
{  
    public List<EnemyData> enemies = new List<EnemyData>();//新建敌人序列
    public string fight_buttton;//开始下一波敌人按钮
}


[Serializable]
public class LevelInfo 
{
    public int levelId;
    public string name;
    public string description;
    public string sceneName;
    public List<EnemyWaves> enemyWaves = new List<EnemyWaves>();//每关敌人数据
    public int InitialCoin = 300;
    public string bgm = "";// 背景音乐名称

}

public class LevelsConfig : ScriptableObject
{

    [Tooltip("一波敌人结束之后,进入下一波的时间间隔")]
    public float GenerateEnemyTimeInterval = 10;

    [Tooltip("玩家血量,默认:20")]
    public int PlayerHp = 20;

    [Tooltip("出售炮塔的折扣,默认:五折")]
    public float SaleDiscount = 0.5f;

    public List<LevelInfo> levels;

    private Dictionary<int,LevelInfo> levelsDictionary = new Dictionary<int,LevelInfo>();//声明私有字典，对关卡检索优化

    public LevelInfo GetLevelInfo(int levelId) {//字典查询
        if (levelsDictionary.Count == 0) {
            foreach (var item in levels)
            {
                if (levelsDictionary.ContainsKey(item.levelId))
                {
                    throw new Exception(string.Format("配表LevelsConfig id:{0}重复,请调整配置!", levelId));
                }
                levelsDictionary.Add(item.levelId, item);
            }
        }

        if(levelsDictionary.ContainsKey(levelId))
        {
            return levelsDictionary[levelId];
        }
        else 
        { 
            return null; 
        }
    }

}
 