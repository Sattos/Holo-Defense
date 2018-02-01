using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : BaseTower {

    public Projectile Missile;

    public EnemyControllerScript.TargetingMode targetingMode;

    protected override bool Attack()
    {
        List<BaseEnemy> target = EnemyControllerScript.Instance.FindEnemiesInRange(transform.position, range, targetCount, targetingMode);
        if(target.Count == 0)
        {
            return false;
        }
        foreach (BaseEnemy tar in target)
        {
            Projectile o = Instantiate(Missile); //new Projectile();
            o.transform.position = transform.position;
            o.Target = tar;
            o.stats = stats;
        }
        
        return true;
    }
}
