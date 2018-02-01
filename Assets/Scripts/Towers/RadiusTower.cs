using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTower : BaseTower {

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
}
