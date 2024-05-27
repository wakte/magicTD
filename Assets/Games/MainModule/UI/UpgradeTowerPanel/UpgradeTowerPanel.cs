using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class UpgradeTowerPanel : Panel
{


    [SerializeField]
    private Text text_upgrade;//升级价格
    [SerializeField]
    private Text text_sell;//出售价格

    private TowerPosition towerPosition;

    [SerializeField]
    private Button btn_upgrade;

    [SerializeField]
    private Transform bg;

    public Action onDisable;


    private TowersController towersController => Module.LoadController<TowersController>();


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        if (param == null || param.Length == 0)
        {
            throw new System.Exception("参数为空!");
        }
           

        towerPosition = (TowerPosition)param[0];

        if (towerPosition == null)
        {
            throw new System.Exception("参数异常!");
        }
            

        InitUI();//初始化UI

        bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//将升级面板重定位
    }

    private void InitUI()
    {

        TowerInfo next = towersController.GetNextLevelTowerInfo(towerPosition.Tower.TowerId);

        btn_upgrade.gameObject.SetActive(next != null);

        if (next != null)
        {

            text_upgrade.text = next.cost.ToString();

            bool isCanUpgrade = towersController.IsCanCraeteTower(next.id);
            btn_upgrade.GetComponent<CanvasGroup>().alpha = isCanUpgrade ? 1 : 0.7f;
        }

        TowerInfo info = towersController.GetTowerInfo(towerPosition.Tower.TowerId);

        float sale_price = info.cost * Module.LoadController<LevelsController>().GetSaleDiscount();

        text_sell.text = ((int)(sale_price)).ToString();

    }

    public void OnBtnUpgradeClick()
    {
        // 下一个等级的炮塔信息
        TowerInfo next = towersController.GetNextLevelTowerInfo(towerPosition.Tower.TowerId);
        bool isCanCreate = towersController.IsCanCraeteTower(next.id);
        if (!isCanCreate)
        {
            // 给一个提示 
            LogUtil.Log("金币不足");
            Module.LoadController<TipController>().ShowToast("金币不足!");
            return;
        }

        towerPosition.Tower.Close();//回收原来旧炮塔的实体
        towerPosition.Tower = towersController.CreateTower(next.id, towerPosition.transform);


        Close();
    }

    public void OnBtnSellClick()
    {
        TowerInfo info = towersController.GetTowerInfo(towerPosition.Tower.TowerId);

        towerPosition.Tower.Close();
        towerPosition.Tower = null;

        int sale_price = (int)(info.cost * Module.LoadController<LevelsController>().GetSaleDiscount());
        Module.LoadController<FightController>().IncreaseCoin(sale_price);


        Close();
    }


    private void Update()
    {
        if (towerPosition != null)
        {
            bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//当相机移动时，更新面板位置
        }
            
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onDisable?.Invoke();
    }

}
