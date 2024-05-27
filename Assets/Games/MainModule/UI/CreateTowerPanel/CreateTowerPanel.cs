using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XFABManager;
using XFGameFramework;

public class CreateTowerPanel : Panel
{

    [SerializeField]
    private Button btn_archer;
    [SerializeField]
    private Button btn_rock;
    [SerializeField]
    private Button btn_magic;
    [SerializeField]
    private Transform bg;//用于控制背景位置

    private TowerPosition towerPosition;


    private TowersController towersController => Module.LoadController<TowersController>();


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        if (param == null || param.Length == 0)
        {
            throw new System.Exception("请传入对应参数!");
        }
            
        towerPosition = (TowerPosition)param[0];//炮塔底座的位置

        if (towerPosition == null)
        {
            throw new System.Exception("参数错误!");
        }

        bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//通过相机将世界坐标转换为屏幕坐标

        InitBtn(btn_archer, TowerType.Archer);//加载按钮UI
        InitBtn(btn_rock, TowerType.Rock);
        InitBtn(btn_magic, TowerType.Magic);
    }



    private void InitBtn(Button button, TowerType towerType)
    {
        TowerInfo info = towersController.FindLevelMinTowerInfo(towerType);
        ImageLoader loader = button.transform.Find("icon").GetComponent<ImageLoader>();//获取按钮上的图标
        Text text_price = button.transform.Find("text_price").GetComponent<Text>();//获取按钮上的文字（金币数量）
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();//用于控制图片透明度

        loader.AssetName = info.icon_name;
        text_price.text = info.cost.ToString();

        bool isCanCreate = towersController.IsCanCraeteTower(info.id);

        canvasGroup.alpha = isCanCreate ? 1 : 0.7f;//调整透明度

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (!isCanCreate)
            {
                // 给一个提示  
                Module.LoadController<TipController>().ShowToast("金币不足!");
                LogUtil.Log("金币不足");
                return;
            }

            // 创建炮塔 
            TowerBase tower = towersController.CreateTower(info.id, towerPosition.transform);
            if (tower != null)
            {
                // 创建成功
                LogUtil.Log("创建成功!");
                towerPosition.Tower = tower;//将创建出来的炮塔对象赋值给相应的底座
                this.Close();
            }
            else
            {
                // 创建失败
                LogUtil.Log("创建失败!");
            }

        });

    }

    private void OnPlayerCoinValueChange(object[] p)
    {

        InitBtn(btn_archer, TowerType.Archer);
        InitBtn(btn_rock, TowerType.Rock);
        InitBtn(btn_magic, TowerType.Magic);
    }


    protected override void Awake()//监听金币变化的事件
    {
        base.Awake();
        ListenerEvents = new EventConfig[] {
            new EventConfig(EventConst.ON_PLAYER_COIN_VALUE_CHANGE,OnPlayerCoinValueChange)
        };
    }

    private void Update()
    {
        if (towerPosition != null)//实时更新屏幕坐标
        {
            bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);
        }
            

    }



}
