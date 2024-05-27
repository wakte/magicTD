using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using XFABManager;
using XFGameFramework;

public class EnemiesController : Controller
{

    private EnemiesConfig EnemiesConfig =>AssetBundleManager.LoadAsset<EnemiesConfig>(Module.ProjectName, "EnemiesConfig");
    //���ݵ���id������·����������
    public Enemy CreateEnemy(int id, SplineContainer path)
    {
        EnemyInfo enemyInfo = GetEnemyInfo(id);
        if (enemyInfo == null)
        {
            throw new System.Exception(string.Format("δ��ѯ��������Ϣ:{0}!", id));
        }
        else
        {
            //ʹ��LoadView��������Ԥ���壬������л�ȡԤ��������
            return Module.LoadView<Enemy>(enemyInfo.prefab_name, null, id, path);
        }
            
        
    }


    //��ѯ������Ϣ
    public EnemyInfo GetEnemyInfo(int id)
    {
        return EnemiesConfig.GetEnemyInfo(id);
    }


}
