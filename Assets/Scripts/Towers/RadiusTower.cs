using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTower : BaseTower {

    public override object GetNextUpgradeStats()
    {
        throw new System.NotImplementedException();
    }

    protected override bool Attack()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range);
        if (enemiesInRange.Length == 0)
        {
            //return false;
        }

        foreach (Collider collider in enemiesInRange)
        {
            BaseEnemy enemy = collider.GetComponent<BaseEnemy>();
            if (enemy != null)
                enemy.Hit(stats);
        }
        return true;
    }

    protected override void SetStats(int level)
    {
        throw new System.NotImplementedException();
    }

    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsMaxLevel()
    {
        throw new System.NotImplementedException();
    }
}
