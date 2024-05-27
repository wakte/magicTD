using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using XFGameFramework;

public class Arrow : View
{

    private Enemy enemy;
    private Vector3 startPosition;
    private int damage;

    private bool isFlying = false;

    private float flyTime = 0.3f;
    private float flyTimer = 0;

    private Vector3 targetPosition;//弓箭落点

    private Spline path = new Spline();//通过spline插件让弓箭以路径的方式飞行

    [SerializeField]
    private Animator animator;


    public void Attack(Enemy enemy, int damage, Vector3 startPosition)
    {
        this.enemy = enemy;
        this.startPosition = startPosition;
        this.damage = damage;

        isFlying = true;
        flyTimer = 0;
        Module.LoadController<AudiosController>().PlaySound("sound_arrow_release_1");
    }


    public void OnDisappearFinsh()
    {
        Close();
    }

    private void Recycle()
    {
        animator.ResetTrigger("disappear");
        animator.SetTrigger("disappear");
    }



    private void Update()
    {
        Fly();
    }

    private void Fly()
    {
        if (!isFlying)
        {
            return;
        }
        if (enemy != null && enemy.gameObject.activeSelf)
        {
            targetPosition = enemy.transform.position + Vector3.up * 0.3f;//获取弓箭落点
        }
        if (enemy != null && enemy.Hp <= 0)
        {
            enemy = null;//消灭敌人
        }

        flyTimer += Time.deltaTime;

        Vector3 dir = targetPosition - startPosition;//水平

        Vector3 center = startPosition + dir * 0.3f;//水平方向上最高点投影
        Vector3 offset = Vector3.Cross(dir, Vector3.forward).normalized * 0.3f;//Vector3.Cross，叉积得到高


        if (dir.y >= 0)//根据弓箭手和敌人位置调整偏移值的方向
        {
            float angle = Vector2.Angle(dir, Vector2.up);
            offset = angle / 90 * offset;
        }
        else
        {
            float angle = Vector2.Angle(dir, Vector2.down);
            offset = angle / 90 * offset;
        }

        path.Clear();

        path.Add(new BezierKnot(startPosition), TangentMode.AutoSmooth);

        if (dir.x < 0)
        {
            path.Add(new BezierKnot(center + offset), TangentMode.AutoSmooth);
        }
        else
        {
            path.Add(new BezierKnot(center - offset), TangentMode.AutoSmooth);
        }


        path.Add(new BezierKnot(targetPosition), TangentMode.AutoSmooth);

        Vector3 d;
        if (dir.x > 0)//在弓箭飞行过程中修改弓箭角度
        {
            d = Vector3.Cross(path.EvaluateUpVector(flyTimer / flyTime), Vector3.forward);
        }
        else
        {
            d = -Vector3.Cross(path.EvaluateUpVector(flyTimer / flyTime), Vector3.forward);
        }
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, d));
        transform.position = path.EvaluatePosition(flyTimer / flyTime);

        if (flyTimer >= flyTime)
        {
            // 对敌人进行攻击
            if (enemy != null)
            {
                enemy.OnHurt(damage, DamageType.Physics);
            }

            isFlying = false;
            Recycle();
            // 播放弓箭音效

            Module.LoadController<AudiosController>().PlaySound("arrow_hit_1");
        }

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isFlying = false;
        enemy = null;
        flyTimer = 0;
    }

}
