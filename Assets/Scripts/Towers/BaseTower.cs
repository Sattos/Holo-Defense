using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private DateTime lastAttack;
    private bool isActive;

    protected abstract bool Attack();

    public void ClickTower()
    {
        //ObjectPlacer.Instance.TurretInfoCanvas.transform.position = transform.position + transform.rotation * new Vector3(0, 0.7f, 0);
        //ObjectPlacer.Instance.TurretInfoCanvas.gameObject.SetActive(true);

        //TurretInfoCanvas.Instance.Activate(this);
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
