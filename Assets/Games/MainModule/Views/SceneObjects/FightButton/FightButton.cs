using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XFGameFramework;

public class FightButton : MonoBehaviour
{

    private Module module;
    //保存按钮的ui便于实时修改其箭头位置
    private FightButtonPanel fightPanel;
    //使用rect来判断开始ui的位置
    private Rect screenRect;
    [SerializeField]
    private float defaultAngle;

    private void Awake()
    {
        screenRect = new Rect(0,0,Screen.width,Screen.height);
    }

    //初始化模块
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

    //更新按钮状态的方法（用于在玩家没有在固定时间内点击下一波时开始下一波）
    public void UpdateProgress(float progress)
    {
        fightPanel.UpdateProgress(progress);    
    }

    //给按钮添加事件的方法
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
        //将场景坐标转化成屏幕坐标
        Vector3 position =  Camera.main.WorldToScreenPoint(transform.position);


        //更新UI箭头的方向
        if (screenRect.Contains(position))
        {
            //更新默认角度
            fightPanel.UpdateDir(defaultAngle);

            fightPanel.SetArrowInactive();

            

        }
        else
        {
            fightPanel.SetArrowActive();    
            //更新计算后的角度
            Vector3 post_position = new Vector3();
            post_position.x = Mathf.Clamp(position.x, 100, Screen.width - 100);
            post_position.y = Mathf.Clamp(position.y, 100, Screen.height - 100);

            Vector3 dir = position - post_position;
            //使用函数计算两点的夹角
            float angle = Vector2.SignedAngle(dir,Vector2.right);
            fightPanel.UpdateDir(-angle);
        }


        //对屏幕坐标的值进行约束
        position.x = Mathf.Clamp(position.x,100,Screen.width-100);
        position.y = Mathf.Clamp(position.y,100,Screen.height-100);   
        fightPanel.transform.position = position;

    }

}
