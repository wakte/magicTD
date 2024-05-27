using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;
public class Archer : View
{
    private Enemy target;//炮塔当前攻击的敌人
    private int damage;

    [SerializeField]
    private Animator animator;//序列化的攻击动画

    private Vector3 right = Vector3.one;
    private Vector3 left = new Vector3(-1, 1, 1);

    [SerializeField]
    private Transform arrow_parent;

    private Arrow arrow = null;

    public void SetTarget(Enemy target)
    {
        this.target = target;
    }

    private bool IsCanAttack()//判断当前是否需要切换攻击动画
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("shot"))
        {
            return false;
        }
        else
        {
            return true;
        }


    }

    public void Attack(int damage)
    {
        if (!IsCanAttack())
        {
            return;
        }

        this.damage = damage;
        animator.ResetTrigger("shot");//控制混合树中的trigger
        animator.SetTrigger("shot");
    }

    public void OnAnimationEvent(string name)
    {
        switch (name)
        {
            case "OnPreAttack"://动画中的事件
                // 把弓箭创建出来
                OnPreAttack();
                break;

            case "OnAttack":
                // 把弓箭发射出去
                OnAttack();
                break;
        }
    }

    private bool WhetherTheTargetIsAbove()
    {

        if (this.target == null)
        {
            return false;
        }

        Vector2 dir = target.transform.position - transform.position;

        if (Vector2.Angle(dir, Vector2.up) < 45)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateLeftRight()
    {

        if (this.target == null)
        {
            return;
        }
        Vector2 dir = target.transform.position - transform.position;
        transform.localScale = dir.x > 0 ? right : left;//设置弓箭手朝向
    }


    public void OnPreAttack()
    {
        // 创建弓箭
        arrow = Module.LoadView<Arrow>(arrow_parent);//根据父物体名称获取弓箭刷新点位

    }

    public void OnAttack()
    {
        if (arrow == null) return;

        if (target != null && target.gameObject.activeSelf)
        {
            arrow.transform.SetParent(null);
            arrow.transform.localScale = Vector3.one;
            arrow.Attack(target, damage, arrow_parent.position);
            arrow = null;
        }
        else//当敌人被消灭时
        {
            // 回收
            arrow.Close();
            arrow = null;
        }
    }


    private void Update()
    {
        animator.SetFloat("up", WhetherTheTargetIsAbove() ? 1 : 0);//根据射手和敌人位置判断射手朝向
        UpdateLeftRight();
    }

}