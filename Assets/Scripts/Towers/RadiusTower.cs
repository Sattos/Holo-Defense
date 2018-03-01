using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTower : BaseTower {

    public Animation hitAnimation;

    private GameObject hitAnimationObject;

    //public class UpgradeStats
    //{
    //    public float damage;
    //    public int attackSpeed;
    //    public float range;
    //    public float damagePerSecond;
    //    public float damageDuration;
    //    public float slow;
    //    public float slowDuration;
    //    public int nextUpgradeCost;
    //    public int sellValue;

    //    public UpgradeStats(float damage, int attackSpeed, float range, float damagePerSecond, float damageDuration, float slow, float slowDuration, int nextUpgradeCost, int sellValue)
    //    {
    //        this.damage = damage;
    //        this.attackSpeed = attackSpeed;
    //        this.range = range;
    //        this.damagePerSecond = damagePerSecond;
    //        this.damageDuration = damageDuration;
    //        this.slow = slow;
    //        this.slowDuration = slowDuration;
    //        this.nextUpgradeCost = nextUpgradeCost;
    //        this.sellValue = sellValue;
    //    }
    //}

    //public static UpgradeStats[] Stats.UpgradeLevels = {
    //    new UpgradeStats(0.5f, 1000, 2, 0, 0, 0.2f, 2, 10, 8),
    //    new UpgradeStats(0.75f, 900, 2.2f, 0, 0, 0.25f, 2, 20, 16),
    //    new UpgradeStats(1, 800, 2.4f, 0, 0, 0.3f, 2.5f, 0, 32)
    //};

    public override void Upgrade()
    {
        if (level >= Stats.UpgradeLevels[towerType].Length)
        {
            return;
        }
        SetStats(++level);
    }

    protected override void SetStats(int val)
    {
        if (val >= Stats.UpgradeLevels[towerType].Length)
        {
            return;
        }

        UpgradeStats upgStats = Stats.UpgradeLevels[towerType][val];
        this.stats = new AttackStats(0, upgStats.damage, upgStats.damagePerSecond, upgStats.damageDuration, upgStats.slowDuration, upgStats.slow, stunDuration, 0, this.gameObject);
        this.range = upgStats.range;
        this.attackSpeed = upgStats.attackSpeed;
        hitAnimationObject.transform.parent.localScale = Vector3.one * upgStats.range;

        this.UpgradeCost = upgStats.nextUpgradeCost;
        this.SellValue = upgStats.sellValue;
    }

    public override object GetNextUpgradeStats()
    {
        if (level >= Stats.UpgradeLevels[towerType].Length)
        {
            return null;
        }
        return Stats.UpgradeLevels[towerType][level + 1];
    }

    public override bool IsMaxLevel()
    {
        return level == Stats.UpgradeLevels[towerType].Length;
    }

    protected override bool Attack()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range/2);
        if (enemiesInRange.Length == 0)
        {
            return false;
        }

        bool enemyHit = false;

        foreach (Collider collider in enemiesInRange)
        {
            BaseEnemy enemy = collider.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.Hit(stats);
                enemyHit = true;
            }
        }

        if (!enemyHit)
            return false;

        hitAnimationObject.GetComponent<Animation>().Play();
        return true;
    }

    public override void FinalizePlacement()
    {
        base.FinalizePlacement();
        hitAnimationObject.transform.position = transform.position;
    }

    protected override void Start()
    {
        GameObject parentObject = Instantiate(new GameObject());
        parentObject.transform.localScale = Vector3.one;
        //parentObject.transform.SetParent(transform);
        hitAnimationObject = Instantiate(hitAnimation.gameObject);
        hitAnimationObject.transform.SetParent(parentObject.transform);
        hitAnimationObject.transform.position = transform.position;
        hitAnimationObject.transform.localScale = Vector3.zero;
        base.Start();
    }
}
