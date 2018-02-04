using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : BaseTower {

    public Projectile Missile;

    public EnemyControllerScript.TargetingMode targetingMode;

    public class UpgradeStats
    {
        public float damage;
        public int attackSpeed;
        public float range;
        public float damagePerSecond;
        public float damageDuration;
        public float slow;
        public float slowDuration;
        public int targetCount;
        public float radius;
        public float velocity;

        public UpgradeStats(float damage, int attackSpeed, float range, float damagePerSecond, float damageDuration, float slow, float slowDuration, int targetCount, float radius, float velocity)
        {
            this.damage = damage;
            this.attackSpeed = attackSpeed;
            this.range = range;
            this.damagePerSecond = damagePerSecond;
            this.damageDuration = damageDuration;
            this.slow = slow;
            this.slowDuration = slowDuration;
            this.targetCount = targetCount;
            this.radius = radius;
            this.velocity = velocity;
        }
    }

    public static UpgradeStats[] UpgradeLevels = {
        new UpgradeStats(1, 700, 2, 0, 0, 0, 0, 1, 0, 0.06f),
        new UpgradeStats(1.5f, 650, 2.2f, 0, 0, 0, 0, 1, 0, 0.07f),
        new UpgradeStats(2, 600, 2.4f, 0, 0, 0, 0, 2, 0, 0.08f)
    };

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

    public override void Upgrade()
    {
        if (level >= UpgradeLevels.Length)
        {
            return;
        }
        SetStats(++level);
    }

    protected override void SetStats(int val)
    {
        if (val >= UpgradeLevels.Length)
        {
            return;
        }

        UpgradeStats upgStats = UpgradeLevels[val];
        this.stats = new AttackStats(upgStats.velocity, upgStats.damage, upgStats.damagePerSecond, upgStats.damageDuration, upgStats.slowDuration, upgStats.slow, stunDuration, upgStats.radius, this.gameObject);
        this.range = upgStats.range;
        this.attackSpeed = upgStats.attackSpeed;
        this.targetCount = upgStats.targetCount;
    }

    public override object GetNextUpgradeStats()
    {
        if(level >= UpgradeLevels.Length)
        {
            return null;
        }
        return UpgradeLevels[level+1];
    }

    public override bool IsMaxLevel()
    {
        return level == UpgradeLevels.Length;
    }
}
