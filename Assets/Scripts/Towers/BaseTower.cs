using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AttackStats
{
    public float speed;
    public float damage;
    public float damagePerSecond;
    public float damageDuration;
    public float slowDuration;
    public float slow;
    public float stunDuration;
    public GameObject source;

    public AttackStats(float speed, float damage, float damagePerSecond, float damageDuration, float slowDuration, float slow, float stunDuration, GameObject source)
    {
        this.speed = speed;
        this.damage = damage;
        this.damagePerSecond = damagePerSecond;
        this.damageDuration = damageDuration;
        this.slowDuration = slowDuration;
        this.slow = slow;
        this.stunDuration = stunDuration;
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
    //temporary

    private DateTime lastAttack;

    protected abstract bool Attack();

    public void ClickTower()
    {
        ObjectPlacer.Instance.TurretInfoCanvas.transform.position = transform.position + transform.rotation * new Vector3(0, 0.7f, 0);
        ObjectPlacer.Instance.TurretInfoCanvas.gameObject.SetActive(true);
    }
    
	// Use this for initialization
	void Start () {
        lastAttack = DateTime.MinValue;
        stats = new AttackStats(speed, damage, damagePerSecond, damageDuration, slowDuration, slow, stunDuration, this.gameObject);
	}
	
	// Update is called once per frame
	protected void Update () {
		if((DateTime.Now - lastAttack).TotalMilliseconds > attackSpeed)
        {
            if(Attack())
                lastAttack = DateTime.Now;
        }
	}
}
