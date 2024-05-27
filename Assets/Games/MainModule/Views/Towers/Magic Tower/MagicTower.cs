using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MagicTower : TowerBase
{


    [SerializeField]
    private MagicLine[] lines;

    [SerializeField]
    private SpriteRenderer[] attack_point;


    private void Awake()
    {
        foreach (MagicLine line in lines)
        {
            line.gameObject.SetActive(false);
        }
    }


    protected override void OnAttack(Enemy enemy, int damage)
    {
        base.OnAttack(enemy, damage);

        foreach (MagicLine line in lines)
        {
            line.Attack(enemy, damage);
        }

    }

    protected override void OnReadying(float t)
    {
        base.OnReadying(t);
        foreach (SpriteRenderer point in attack_point) {
            point.color = new Color(1, 1, 1, t);
        }
    }

}
