using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneTower : TowerBase
{

    [SerializeField]
    private Animator animator;//获取动画状态机对象

    private Enemy target;
    private int damage;

    [SerializeField]
    private Transform stone_pos;//生成石头实体的位置

    private Stone stone;

    [SerializeField]
    private Animator animtor_ready;


    protected override void OnAttack(Enemy enemy, int damage)
    {
        base.OnAttack(enemy, damage);

        this.target = enemy;
        this.damage = damage;


        // 播放攻击动画

        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
    }

    protected override void OnReadying(float t)
    {
        base.OnReadying(t);

        animtor_ready.gameObject.SetActive(true);
        animtor_ready.Play("explode", 0, 1 - t);

    }


    protected override void OnReady()
    {
        base.OnReady();

        animtor_ready.gameObject.SetActive(false);

        // 创建石头
        stone = Module.LoadView<Stone>(stone_pos);

    }

    public void OnAnimationAttack() 
    {
        // 石头发射控制

        if (stone == null)
        {
            return;
        }
        if (target != null)
        {
            stone.Attack(target, this.damage);
        }
            
        else
        {
            stone.Close();
        }
          
        stone = null;

    }


    protected override void OnDisable()
    {
        base.OnDisable();
        if (stone != null) { 
            stone .Close();
            stone = null;
        }
    }


}
