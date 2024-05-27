using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using XFABManager;
using XFGameFramework;

public class Stone : View
{

    private bool isFlying = false;
    private float flyTimer = 0;//飞行时间

    private float flyTime = 0.3f;//石头飞行的时间阈值

    private Enemy target;//攻击敌人
    private int damage;

    private Vector3 startPosition;

    private Vector3 targetPosition;

    private Spline path = new Spline();


    private Collider2D[] Enemys = new Collider2D[100];//


    public void Attack(Enemy enemy,int damage) 
    {
        transform.SetParent(null);

        this.target = enemy;
        this.damage = damage;

        startPosition = transform.position;

        isFlying = true;
        flyTimer = 0;

    }

    public void OnExplode() {

        for (int i = 0; i < Enemys.Length; i++)
        {
            Enemys[i] = null;
        }

        // 对附近造成伤害

        Physics2D.OverlapCircleNonAlloc(transform.position, 0.6f, Enemys);

        for (int i = 0; i < Enemys.Length; i++)
        {
            if (Enemys[i] == null)
            {
                continue;
            }
            Enemy enemy = Enemys[i].GetComponent<Enemy>();
            if (enemy == null)
            {
                continue;
            }
            enemy.OnHurt(this.damage, DamageType.Physics);
        }

        

        // 加载爆炸的动画
        GameObject obj = GameObjectLoader.Load(Module.ProjectName, "StoneExplodeAnim");
        obj.transform.position = transform.position;

        // 回收
        TimerManager.DelayInvoke(() =>
        {
            GameObjectLoader.UnLoad(obj);
        },0.3f);

        Close();
    }


    private void Update()
    {
        if (!isFlying)
        {
            return;
        }

        path.Clear();

        // 添加三个点，用于绘制石头飞行路径

        if (target != null  && target.gameObject.activeSelf)
        {
            targetPosition = target.transform.position;//石头落点位置
        }

        if (target != null && target.Hp <= 0)
        {
            target = null;
        }

        Vector3 dir = targetPosition - startPosition;
         
        Vector3 center = startPosition + dir * 0.5f;

        center.y = 0;//在绘制路径时要忽略，否则石头飞行的轨迹会扭动

        Vector3 offset;

        if (dir.y > 0)
        {
            offset = center + new Vector3(0, targetPosition.y + 1, 0);
        }
        else {
            offset = center + new Vector3(0, startPosition.y + 1, 0);
        }

        path.Add(new BezierKnot(startPosition), TangentMode.AutoSmooth);
        path.Add(new BezierKnot(offset), TangentMode.AutoSmooth);
        path.Add(new BezierKnot(targetPosition), TangentMode.AutoSmooth);

        flyTimer += Time.deltaTime;

        transform.position = path.EvaluatePosition(flyTimer / flyTime);

        if (flyTimer >= flyTime) {
            isFlying = false;
            flyTimer = 0;

            // 对敌人造成伤害
            OnExplode();
        }

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isFlying = false;
        flyTimer = 0;
        target = null;
    }

}
