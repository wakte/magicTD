using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework;

public class TowersController : Controller
{

    private TowersConfig TowersConfig => AssetBundleManager.LoadAsset<TowersConfig>(Module.ProjectName, "TowersConfig");

    private List<TowerBase> towers = new List<TowerBase>();//存放某一类型所有等级的炮塔

    // Fix编码

    public TowerInfo GetTowerInfo(int towerId)
    {
        return TowersConfig.GetTowerInfo(towerId);
    }

    public bool IsCanCraeteTower(int towerId)
    {
        TowerInfo towerInfo = GetTowerInfo(towerId);
        int playerCoin = Module.LoadController<FightController>().GetFightModel().PlayerCoin;

        return towerInfo.cost <= playerCoin;
    }

    public TowerBase CreateTower(int towerId, Transform parent)
    {
        TowerInfo towerInfo = GetTowerInfo(towerId);
        if (!IsCanCraeteTower(towerId))
        {
            return null;
        }

        TowerBase towerBase = Module.LoadView<TowerBase>(towerInfo.prefab_name, parent, towerId);//金币足够，加载炮塔

        LogUtil.Log("资源名:{0} towerBase:{1}", towerInfo.prefab_name, towerBase == null);

        // 扣除金币  
        Module.LoadController<FightController>().ReduceCoin(towerInfo.cost);

        if (!towers.Contains(towerBase))
        {
            towers.Add(towerBase);
        }
            
        // 播放创建炮塔的音效

       // Module.LoadController<AudiosController>().PlaySound(AudioConst.sound_tower_build);

        return towerBase;
    }


    public List<TowerInfo> GetTowerInfoByType(TowerType type)
    {
        return TowersConfig.GetTowersInfoByType(type);//根据炮塔类型获取所有等级的炮塔
    }


    public TowerInfo FindLevelMinTowerInfo(TowerType type)
    {
        // 找到某个类型的所有炮塔 
        List<TowerInfo> towers = GetTowerInfoByType(type);

        TowerInfo min = null;

        foreach (var item in towers)
        {
            if (min == null)
            {
                min = item;
                continue;
            }

            if (min.level > item.level)
            {
                min = item;
            }
                
        }

        return min;
    }

    public TowerInfo FindLevelMaxTowerInfo(TowerType type)
    {
        // 找到某个类型的所有炮塔 
        List<TowerInfo> towers = GetTowerInfoByType(type);

        TowerInfo max = null;

        foreach (var item in towers)
        {
            if (max == null)
            {
                max = item;
                continue;
            }

            if (max.level < item.level)
            {
                max = item;
            }
               
        }

        return max;
    }

    public TowerInfo GetNextLevelTowerInfo(int towerId)
    {
        TowerInfo towerInfo = GetTowerInfo(towerId);

        if (towerInfo == null)
        {
            throw new System.Exception(string.Format("炮塔信息查询失败:{0}", towerId));
        }
            

        List<TowerInfo> towers = GetTowerInfoByType(towerInfo.type);

        foreach (var item in towers)//遍历这个类型所有的炮塔
        {
            if (item.level - towerInfo.level == 1)
            {
                return item;
            }
                
        }

        return null;
    }

    public void ClearTowers()
    {

        while (towers.Count > 0)
        {
            TowerBase tower = towers[towers.Count - 1];
            if (tower != null)
            {
                tower.Close();
            }
            towers.RemoveAt(towers.Count - 1);
        }
        // 清空弓箭
        Module.ClearView<Arrow>();
        // 清空石头
        Module.ClearView<Stone>();

    }

}
