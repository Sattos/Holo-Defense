using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTower : BaseTower {

    public Projectile Missile;

    public GameObject ProjectileSource;
    public GameObject RotatingObject;
    public bool rotate;

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
        public int nextUpgradeCost;
        public int sellValue;

        public UpgradeStats(float damage, int attackSpeed, float range, float damagePerSecond, float damageDuration, float slow, float slowDuration, int targetCount, float radius, float velocity, int cost, int sellValue)
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
            this.nextUpgradeCost = cost;
            this.sellValue = sellValue;
        }
    }

    public static UpgradeStats[] UpgradeLevels = {
        new UpgradeStats(1, 700, 2, 0, 0, 0, 0, 1, 0, 0.06f, 10, 8),
        new UpgradeStats(1.5f, 650, 2.2f, 0, 0, 0, 0, 1, 0.6f, 0.07f, 30, 16),
        new UpgradeStats(2, 600, 2.4f, 0, 0, 0, 0, 2, 0.7f, 0.08f, 0, 40)
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
            if (rotate)
            {
                RotatingObject.transform.LookAt(tar.transform, transform.up);
                Vector3 rotation = RotatingObject.transform.localRotation.eulerAngles;
                rotation.x = 0;
                rotation.z = 0;
                RotatingObject.transform.localRotation = Quaternion.Euler(rotation);
            }
            Projectile o = Instantiate(Missile); //new Projectile();
            o.transform.position = ProjectileSource.transform.position;
            Debug.Log("source" +ProjectileSource.transform.position);
            Debug.Log("projectile" +o.transform.position);
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

        this.UpgradeCost = upgStats.nextUpgradeCost;
        this.SellValue = upgStats.sellValue;
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
