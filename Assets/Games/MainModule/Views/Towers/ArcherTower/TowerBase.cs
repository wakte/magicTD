using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class TowerBase : View
{
    // Fix编码


    [SerializeField]
    protected List<Enemy> enemies;//炮塔范围内所有敌人

    [SerializeField]
    protected Transform range_parent;//攻击范围
    [SerializeField]
    protected SpriteRenderer rangeRenderer;//用于显示炮塔攻击范围

    private float readyTimer = 0;//炮塔攻击间隔计时
    private bool isReady = false;

    public int TowerId { get; private set; }//保存当前炮塔id

    private TowerInfo towerInfo;//当前炮塔配制信息


    public override void OnLoaded(params object[] param)//加载炮塔
    {
        base.OnLoaded(param);
        if (param == null || param.Length == 0)
        {
            throw new System.Exception("请传入炮塔id!");
        }

        TowerId = (int)param[0];
        towerInfo = Module.LoadController<TowersController>().GetTowerInfo(TowerId);
        if (towerInfo == null)
        {
            throw new System.Exception(string.Format("查询TowerInfo失败,id:{0}", TowerId));
        }
        range_parent.transform.localScale = Vector3.one * towerInfo.attack_range;//设置攻击范围

        readyTimer = 0;
        isReady = false;

        HideAttackRange();
    }


    public void _OnTriggerEnter2D(Collider2D collision)//敌人进入范围
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }
        if (!enemies.Contains(enemy))//判断有没有重复添加敌人
        {
            enemies.Add(enemy);
        }

    }


    public void _OnTriggerExit2D(Collider2D collision)//敌人走出范围
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }

        if (enemies.Count == 0)
        {
            OnAttackEnd();
        }

    }

    protected virtual void OnAttackEnd()//攻击结束回调
    {
    }

    protected virtual void OnAttack(Enemy enemy, int damage) { }//虚方法，在具体的炮塔控制类中会重写

    protected virtual void OnReadying(float t) { }

    protected virtual void OnReady()
    {

    }

    public bool IsCanAttack()
    {

        if (enemies == null || enemies.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    public void ShowAttackRange()
    {
        rangeRenderer.enabled = true;
    }

    public void HideAttackRange()
    {
        rangeRenderer.enabled = false;
    }





    private void Update()
    {

        if (isReady)
        {
            // 可以对敌人进行攻击
            if (IsCanAttack())
            {
                int damage = Random.Range(towerInfo.damage_min, towerInfo.damage_max + 1);//攻击伤害
                OnAttack(enemies[0], damage);//攻击
                isReady = false;//重置冷却时间
                readyTimer = 0;
            }
        }
        else
        {
            OnReadying(readyTimer / towerInfo.cooling_time);//准备过程中回调
            readyTimer += Time.deltaTime;//开始冷却计时
            if (readyTimer >= towerInfo.cooling_time)
            {
                readyTimer = 0;
                isReady = true;
                OnReady();//准备完成回调
            }

        }

    }


}
