using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : BaseTower {

    public Projectile Missile;

    protected override bool Attack()
    {
        BaseEnemy target = EnemyControllerScript.Instance.FindFarthestEnemyInRange(transform.position, range);
        if(target == null)
        {
            return false;
        }
        Projectile o = Instantiate(Missile); //new Projectile();
        o.transform.position = transform.position;
        o.Target = target;
        o.stats = stats;
        
        return true;
    }
}
