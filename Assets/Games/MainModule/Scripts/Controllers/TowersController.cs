using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFABManager;
using XFGameFramework;

public class TowersController : Controller
{

    private TowersConfig TowersConfig => AssetBundleManager.LoadAsset<TowersConfig>(Module.ProjectName, "TowersConfig");

    private List<TowerBase> towers = new List<TowerBase>();//���ĳһ�������еȼ�������

    // Fix����

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

        TowerBase towerBase = Module.LoadView<TowerBase>(towerInfo.prefab_name, parent, towerId);//����㹻����������

        LogUtil.Log("��Դ��:{0} towerBase:{1}", towerInfo.prefab_name, towerBase == null);

        // �۳����  
        Module.LoadController<FightController>().ReduceCoin(towerInfo.cost);

        if (!towers.Contains(towerBase))
        {
            towers.Add(towerBase);
        }
            
        // ���Ŵ�����������Ч

       // Module.LoadController<AudiosController>().PlaySound(AudioConst.sound_tower_build);

        return towerBase;
    }


    public List<TowerInfo> GetTowerInfoByType(TowerType type)
    {
        return TowersConfig.GetTowersInfoByType(type);//�����������ͻ�ȡ���еȼ�������
    }


    public TowerInfo FindLevelMinTowerInfo(TowerType type)
    {
        // �ҵ�ĳ�����͵��������� 
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
        // �ҵ�ĳ�����͵��������� 
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
            throw new System.Exception(string.Format("������Ϣ��ѯʧ��:{0}", towerId));
        }
            

        List<TowerInfo> towers = GetTowerInfoByType(towerInfo.type);

        foreach (var item in towers)//��������������е�����
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
        // ��չ���
        Module.ClearView<Arrow>();
        // ���ʯͷ
        Module.ClearView<Stone>();

    }

}
