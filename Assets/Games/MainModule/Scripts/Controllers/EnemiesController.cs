using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using XFABManager;
using XFGameFramework;

public class EnemiesController : Controller
{

    private EnemiesConfig EnemiesConfig =>AssetBundleManager.LoadAsset<EnemiesConfig>(Module.ProjectName, "EnemiesConfig");
    //根据敌人id和所走路径创建敌人
    public Enemy CreateEnemy(int id, SplineContainer path)
    {
        EnemyInfo enemyInfo = GetEnemyInfo(id);
        if (enemyInfo == null)
        {
            throw new System.Exception(string.Format("未查询到敌人信息:{0}!", id));
        }
        else
        {
            //使用LoadView方法加载预制体，从配表中获取预制体名称
            return Module.LoadView<Enemy>(enemyInfo.prefab_name, null, id, path);
        }
            
        
    }


    //查询敌人信息
    public EnemyInfo GetEnemyInfo(int id)
    {
        return EnemiesConfig.GetEnemyInfo(id);
    }


}
