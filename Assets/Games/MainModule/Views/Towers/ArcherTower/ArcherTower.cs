using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherTower : TowerBase
{
    // Fix编码

    [SerializeField]
    private Archer[] archers;


    protected override void OnAttack(Enemy enemy, int damage)
    {
        base.OnAttack(enemy, damage);

        foreach (var archer in archers)
        {
            archer.SetTarget(enemy);
            archer.Attack(damage);
        }

    }

    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
        foreach (var archer in archers)
        {
            archer.SetTarget(null); 
        }
    }

}
