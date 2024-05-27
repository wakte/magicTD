using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework;



public class LevelsController : Controller
{

    private LevelsConfig LevelsConfig => AssetBundleManager.LoadAsset<LevelsConfig>(Module.ProjectName, "LevelsConfig");//������Դģ�������عؿ����

    public LevelInfo GetLevelInfo(int levelId)
    {
        //foreach(var item in LevelsConfig.levels)//���������ݹؿ�id�ҵ��ؿ�����(���Ż�)
        //{
        //    if (item.levelId == levelId)
        //    {
        //        return item;
        //    }
        //}
        return LevelsConfig.GetLevelInfo(levelId);
    }
        
    public List<LevelInfo> GetLevelsByPages(int page,int size)//
    {
        if (page < 1)
        {
            throw new System.Exception("ҳ������Ӧ�ôӵ�һҳ��ʼ");
        }
        if (size <= 0)
        {
            throw new System.Exception("ÿҳ�Ĺؿ���С��0");
        }
        List<LevelInfo> levels = new List<LevelInfo>();

        for (int i = (page - 1)*size;i<page*size;i++)
        {
            if (i >= LevelsConfig.levels.Count)
            {
                break;
            }
            levels.Add(LevelsConfig.levels[i]); //��ӵ�ǰҳ�Ĺؿ�
        }

        return levels;
    }

    public bool IsLastPage(int page,int size)
    {
        return page*size>=LevelsConfig.levels.Count;
    }

    public int GetLevelCount()
    {
        return LevelsConfig.levels.Count;
    }
    public int GetPlayerHp()
    {
        return LevelsConfig.PlayerHp;
    }


    public float GetGenerateEnemyTimeInterval()
    {//��ȡ��������ʱ����
        return LevelsConfig.GenerateEnemyTimeInterval;
    }

    public float GetSaleDiscount()
    {//��ȡ���ۼ۸�
        return LevelsConfig.SaleDiscount;
    }

}
