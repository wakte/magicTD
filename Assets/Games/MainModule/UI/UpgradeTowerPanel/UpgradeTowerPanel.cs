using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFGameFramework;

public class UpgradeTowerPanel : Panel
{


    [SerializeField]
    private Text text_upgrade;//�����۸�
    [SerializeField]
    private Text text_sell;//���ۼ۸�

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
            throw new System.Exception("����Ϊ��!");
        }
           

        towerPosition = (TowerPosition)param[0];

        if (towerPosition == null)
        {
            throw new System.Exception("�����쳣!");
        }
            

        InitUI();//��ʼ��UI

        bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//����������ض�λ
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
        // ��һ���ȼ���������Ϣ
        TowerInfo next = towersController.GetNextLevelTowerInfo(towerPosition.Tower.TowerId);
        bool isCanCreate = towersController.IsCanCraeteTower(next.id);
        if (!isCanCreate)
        {
            // ��һ����ʾ 
            LogUtil.Log("��Ҳ���");
            Module.LoadController<TipController>().ShowToast("��Ҳ���!");
            return;
        }

        towerPosition.Tower.Close();//����ԭ����������ʵ��
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
            bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//������ƶ�ʱ���������λ��
        }
            
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onDisable?.Invoke();
    }

}
