using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class MagicLine : MonoBehaviour
{

    public void Attack(Enemy enemy,int damage) {

        gameObject.SetActive(true);

        Vector3 dir = enemy.transform.position - transform.position + Vector3.up * 0.3f;//炮塔和敌人的向量
        transform.eulerAngles = new Vector3(0, 0,-Vector2.SignedAngle(dir, Vector2.up));//更新闪电实体的角度
        transform.localScale = new Vector3(1, Vector2.Distance(transform.position ,enemy.transform.position), 1);
        enemy.OnHurt(damage, DamageType.Magic);

        TimerManager.DelayInvoke(() => {
            gameObject.SetActive(false);
        }, 0.06f);
    }

}
