using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseTower : MonoBehaviour {


    public int level;
    public float baseCost;
    public float totalCost;
    public float damage;
    public float range;
    public int attackSpeed;
    public int targetCount;
    public float radius;
    public float damagePerSecond;
    public float damageDuration;
    public float slowDuration;
    public float slow;
    public float stunDuration;
    public bool targetGround;
    public bool targetFlying;

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
	}
	
	// Update is called once per frame
	protected void Update () {
		if((DateTime.Now - lastAttack).Milliseconds > attackSpeed)
        {
            if(Attack())
                lastAttack = DateTime.Now;
        }
	}
}
