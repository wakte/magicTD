using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XFGameFramework;

public class FightButton : MonoBehaviour
{

    private Module module;
    //���水ť��ui����ʵʱ�޸����ͷλ��
    private FightButtonPanel fightPanel;
    //ʹ��rect���жϿ�ʼui��λ��
    private Rect screenRect;
    [SerializeField]
    private float defaultAngle;

    private void Awake()
    {
        screenRect = new Rect(0,0,Screen.width,Screen.height);
    }

    //��ʼ��ģ��
    public void Init(Module module)
    {
        this.module = module;
    }
    public void Show()
    {
        //
        gameObject.SetActive(true);    
        if (fightPanel!=null)
        {
            fightPanel.Close();
            fightPanel = null;
        }
        fightPanel = module.LoadPanel<FightButtonPanel>();

    }

    public void Hide()
    {
        if (fightPanel != null)
        {
            fightPanel.Close();
            fightPanel = null;
        }
        gameObject.SetActive(false);
    }

    //���°�ť״̬�ķ��������������û���ڹ̶�ʱ���ڵ����һ��ʱ��ʼ��һ����
    public void UpdateProgress(float progress)
    {
        fightPanel.UpdateProgress(progress);    
    }

    //����ť����¼��ķ���
    public void AddClick(Action action)
    {
        fightPanel.onClick += action;
    }

    public void Update()
    {
        if(fightPanel==null)
        {
            return;
        }
        //����������ת������Ļ����
        Vector3 position =  Camera.main.WorldToScreenPoint(transform.position);


        //����UI��ͷ�ķ���
        if (screenRect.Contains(position))
        {
            //����Ĭ�ϽǶ�
            fightPanel.UpdateDir(defaultAngle);

            fightPanel.SetArrowInactive();

            

        }
        else
        {
            fightPanel.SetArrowActive();    
            //���¼����ĽǶ�
            Vector3 post_position = new Vector3();
            post_position.x = Mathf.Clamp(position.x, 100, Screen.width - 100);
            post_position.y = Mathf.Clamp(position.y, 100, Screen.height - 100);

            Vector3 dir = position - post_position;
            //ʹ�ú�����������ļн�
            float angle = Vector2.SignedAngle(dir,Vector2.right);
            fightPanel.UpdateDir(-angle);
        }


        //����Ļ�����ֵ����Լ��
        position.x = Mathf.Clamp(position.x,100,Screen.width-100);
        position.y = Mathf.Clamp(position.y,100,Screen.height-100);   
        fightPanel.transform.position = position;

    }

}
