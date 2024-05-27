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
    private Transform bg;//���ڿ��Ʊ���λ��

    private TowerPosition towerPosition;


    private TowersController towersController => Module.LoadController<TowersController>();


    public override void OnLoaded(params object[] param)
    {
        base.OnLoaded(param);

        if (param == null || param.Length == 0)
        {
            throw new System.Exception("�봫���Ӧ����!");
        }
            
        towerPosition = (TowerPosition)param[0];//����������λ��

        if (towerPosition == null)
        {
            throw new System.Exception("��������!");
        }

        bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);//ͨ���������������ת��Ϊ��Ļ����

        InitBtn(btn_archer, TowerType.Archer);//���ذ�ťUI
        InitBtn(btn_rock, TowerType.Rock);
        InitBtn(btn_magic, TowerType.Magic);
    }



    private void InitBtn(Button button, TowerType towerType)
    {
        TowerInfo info = towersController.FindLevelMinTowerInfo(towerType);
        ImageLoader loader = button.transform.Find("icon").GetComponent<ImageLoader>();//��ȡ��ť�ϵ�ͼ��
        Text text_price = button.transform.Find("text_price").GetComponent<Text>();//��ȡ��ť�ϵ����֣����������
        CanvasGroup canvasGroup = button.GetComponent<CanvasGroup>();//���ڿ���ͼƬ͸����

        loader.AssetName = info.icon_name;
        text_price.text = info.cost.ToString();

        bool isCanCreate = towersController.IsCanCraeteTower(info.id);

        canvasGroup.alpha = isCanCreate ? 1 : 0.7f;//����͸����

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (!isCanCreate)
            {
                // ��һ����ʾ  
                Module.LoadController<TipController>().ShowToast("��Ҳ���!");
                LogUtil.Log("��Ҳ���");
                return;
            }

            // �������� 
            TowerBase tower = towersController.CreateTower(info.id, towerPosition.transform);
            if (tower != null)
            {
                // �����ɹ�
                LogUtil.Log("�����ɹ�!");
                towerPosition.Tower = tower;//��������������������ֵ����Ӧ�ĵ���
                this.Close();
            }
            else
            {
                // ����ʧ��
                LogUtil.Log("����ʧ��!");
            }

        });

    }

    private void OnPlayerCoinValueChange(object[] p)
    {

        InitBtn(btn_archer, TowerType.Archer);
        InitBtn(btn_rock, TowerType.Rock);
        InitBtn(btn_magic, TowerType.Magic);
    }


    protected override void Awake()//������ұ仯���¼�
    {
        base.Awake();
        ListenerEvents = new EventConfig[] {
            new EventConfig(EventConst.ON_PLAYER_COIN_VALUE_CHANGE,OnPlayerCoinValueChange)
        };
    }

    private void Update()
    {
        if (towerPosition != null)//ʵʱ������Ļ����
        {
            bg.transform.position = Camera.main.WorldToScreenPoint(towerPosition.transform.position);
        }
            

    }



}
