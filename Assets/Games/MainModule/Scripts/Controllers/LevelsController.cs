using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework;



public class LevelsController : Controller
{

    private LevelsConfig LevelsConfig => AssetBundleManager.LoadAsset<LevelsConfig>(Module.ProjectName, "LevelsConfig");//传入资源模块名加载关卡配表

    public LevelInfo GetLevelInfo(int levelId)
    {
        //foreach(var item in LevelsConfig.levels)//遍历，根据关卡id找到关卡数据(被优化)
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
            throw new System.Exception("页数出错，应该从第一页开始");
        }
        if (size <= 0)
        {
            throw new System.Exception("每页的关卡数小于0");
        }
        List<LevelInfo> levels = new List<LevelInfo>();

        for (int i = (page - 1)*size;i<page*size;i++)
        {
            if (i >= LevelsConfig.levels.Count)
            {
                break;
            }
            levels.Add(LevelsConfig.levels[i]); //添加当前页的关卡
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
    {//获取敌人生成时间间隔
        return LevelsConfig.GenerateEnemyTimeInterval;
    }

    public float GetSaleDiscount()
    {//获取出售价格
        return LevelsConfig.SaleDiscount;
    }

}
