using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;

public class AttackStats
{
    public float velocity;
    public float damage;
    public float damagePerSecond;
    public float damageDuration;
    public float slowDuration;
    public float slow;
    public float stunDuration;
    public float areaOfEffect;
    public GameObject source;

    public AttackStats(float velocity, float damage, float damagePerSecond, float damageDuration, float slowDuration, float slow, float stunDuration, float areaOfEffect, GameObject source)
    {
        this.velocity = velocity;
        this.damage = damage;
        this.damagePerSecond = damagePerSecond;
        this.damageDuration = damageDuration;
        this.slowDuration = slowDuration;
        this.slow = slow;
        this.stunDuration = stunDuration;
        this.areaOfEffect = areaOfEffect;
        this.source = source;
    }
}

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

    //ProjectileTower
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

    //RadiusTower
    public UpgradeStats(float damage, int attackSpeed, float range, float damagePerSecond, float damageDuration, float slow, float slowDuration, int nextUpgradeCost, int sellValue)
    {
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        this.range = range;
        this.damagePerSecond = damagePerSecond;
        this.damageDuration = damageDuration;
        this.slow = slow;
        this.slowDuration = slowDuration;
        this.nextUpgradeCost = nextUpgradeCost;
        this.sellValue = sellValue;
    }

    public static UpgradeStats[] ArcherUpgradeLevels = {
        new UpgradeStats(2, 700, 2, 0, 0, 0, 0, 1, 0, 0.06f, 10, 8),
        new UpgradeStats(3.5f, 650, 2.2f, 0, 0, 0, 0, 1, 0, 0.07f, 30, 16),
        new UpgradeStats(6, 500, 2.6f, 0, 0, 0, 0, 1, 0, 0.08f, 0, 40)
    };

    public static UpgradeStats[] CannonUpgradeLevels = {
        new UpgradeStats(3.0f, 1500, 2.4f, 0, 0, 0, 0, 1, 0, 0.06f, 10, 8),
        new UpgradeStats(5.0f, 1250, 2.6f, 0, 0, 0, 0, 1, 0.6f, 0.07f, 30, 16),
        new UpgradeStats(8.5f, 1200, 3.0f, 0, 0, 0, 0, 1, 1.5f, 0.08f, 0, 40)
    };

    public static UpgradeStats[] MageUpgradeLevels = {
        new UpgradeStats(0.5f, 1000, 2, 0, 0, 0.2f, 2, 10, 8),
        new UpgradeStats(0.75f, 900, 2.2f, 0, 0, 0.25f, 2, 20, 16),
        new UpgradeStats(1, 800, 2.4f, 0, 0, 0.3f, 2.5f, 0, 32)
    };
}

public enum TowerType
{
    Archer,
    Cannon,
    Mage,
    Last
}

public static class Stats
{
    public static Dictionary<TowerType, UpgradeStats[]> UpgradeLevels = new Dictionary<TowerType, UpgradeStats[]>() {
        {TowerType.Archer, UpgradeStats.ArcherUpgradeLevels},
        {TowerType.Cannon, UpgradeStats.CannonUpgradeLevels},
        {TowerType.Mage, UpgradeStats.MageUpgradeLevels}
    };

    
}

public abstract class BaseTower : MonoBehaviour {
    public int level;
    public float baseCost;
    public float totalCost;
    public float range;
    public int attackSpeed;
    public int targetCount;
    public bool targetGround;
    public bool targetFlying;

    public AttackStats stats;

    //temporary
    public float speed;
    public float damage;
    public float damagePerSecond;
    public float damageDuration;
    public float slowDuration;
    public float slow;
    public float stunDuration;
    public float areaOfEffect;
    //temporary

    public int UpgradeCost;// { get; protected set; }
    public int SellValue;// { get; protected set; }

    private DateTime lastAttack;
    private bool isActive;

    public TowerType towerType;

   

    protected abstract bool Attack();

    public void ClickTower()
    {
        //ObjectPlacer.Instance.TurretInfoCanvas.transform.position = transform.position + transform.rotation * new Vector3(0, 0.7f, 0);
        //ObjectPlacer.Instance.TurretInfoCanvas.gameObject.SetActive(true);

        //TurretInfoCanvas.Instance.Activate(this);
        AppState.Instance.TurretInfoCanvas.Activate(this);
    }

    public abstract void Upgrade();

    protected abstract void SetStats(int level);

    public abstract object GetNextUpgradeStats();

    public abstract bool IsMaxLevel();

    public virtual void FinalizePlacement()
    {
        isActive = true;
    }
    
	// Use this for initialization
	protected virtual void Start () {
        lastAttack = DateTime.MinValue;
        //stats = new AttackStats(speed, damage, damagePerSecond, damageDuration, slowDuration, slow, stunDuration, areaOfEffect, this.gameObject);
        SetStats(0);
	}
	
	// Update is called once per frame
	protected virtual void Update () {
        if (!isActive)
            return;

		if((DateTime.Now - lastAttack).TotalMilliseconds > attackSpeed)
        {
            if(Attack())
                lastAttack = DateTime.Now;
        }
	}
}
