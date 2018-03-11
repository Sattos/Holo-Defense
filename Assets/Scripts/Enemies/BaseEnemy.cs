using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public float duration;
    public float value;

    public Effect(float duration, float value)
    {
        this.duration = duration;
        this.value = value;
    }
}

public class BaseEnemy : MonoBehaviour {

    public float health;
    public float speed;
    public int money;
    public BaseStats stats;

    public float currentSlow;
    public float currentDamageOverTime;

    private bool hasHitBase;

    private Dictionary<GameObject, Effect> slowEffects = new Dictionary<GameObject, Effect>();
    private Dictionary<GameObject, Effect> damageOverTimeEffects = new Dictionary<GameObject, Effect>();

    public struct BaseStats
    {
        public float health;
        public float speed;
        public int money;
        public bool isFlying;

        public BaseStats(float health, float speed, int money, bool isFlying = false)
        {
            this.health = health;
            this.speed = speed;
            this.money = money;
            this.isFlying = isFlying;
        }
    }

    public void Hit(AttackStats stats)
    {
        health -= stats.damage;
        if (health < 0)
        {
            EnemyControllerScript.Instance.DestroyEnemy(this);
            return;
        }
        if (stats.damageDuration > 0)
        {
            if (damageOverTimeEffects.ContainsKey(stats.source))
            {
                damageOverTimeEffects[stats.source].duration = stats.damageDuration;
                damageOverTimeEffects[stats.source].value = stats.damagePerSecond;
            }
            else
            {
                damageOverTimeEffects.Add(stats.source, new Effect(stats.damageDuration, stats.damagePerSecond));
            }
        }
        if (stats.slowDuration > 0)
        {
            if (slowEffects.ContainsKey(stats.source))
            {
                slowEffects[stats.source].duration = stats.slowDuration;
                slowEffects[stats.source].value = stats.slow;
            }
            else
            {
                slowEffects.Add(stats.source, new Effect(stats.slowDuration, stats.slow));
            }
        }
    }

    public void DamageOverTime()
    {
        currentDamageOverTime = 0;
        if (damageOverTimeEffects.Count == 0)
        {
            return;
        }
        List<GameObject> list = new List<GameObject>(damageOverTimeEffects.Keys);
        foreach (GameObject key in list)
        {
            currentDamageOverTime += damageOverTimeEffects[key].value;
            damageOverTimeEffects[key].duration -= Time.deltaTime;
            if (damageOverTimeEffects[key].duration <= 0)
            {
                {
                    damageOverTimeEffects.Remove(key);
                }
            }
        }
        health -= currentDamageOverTime * Time.deltaTime;
        if (health < 0)
        {
            EnemyControllerScript.Instance.DestroyEnemy(this);
        }
    }

    public void Slow()
    {
        if (slowEffects.Count == 0)
        {
            currentSlow = 1;
            return;
        }
        List<GameObject> list = new List<GameObject>(slowEffects.Keys);
        foreach (GameObject key in list)
        {
            if (currentSlow > slowEffects[key].value)
            {
                currentSlow = 1.0f - slowEffects[key].value;
            }
            slowEffects[key].duration -= Time.deltaTime;
            if (slowEffects[key].duration <= 0)
            {
                {
                    slowEffects.Remove(key);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Base")
        {
            hasHitBase = true;
            EnemyControllerScript.Instance.DamageBase();
            EnemyControllerScript.Instance.DestroyEnemy(this);
        }
    }

    private void OnDestroy()
    {
        if (!hasHitBase)
        {
            AppState.Instance.money += stats.money;
            AppState.Instance.UpdateMoneyText();
        }
    }

    // Use this for initialization
    void Start () {
        stats = new BaseStats(health, speed, money);
    }
	
	// Update is called once per frame
	void Update () {
        DamageOverTime();
        Slow();
        transform.position = Vector3.MoveTowards(transform.position, EnemyControllerScript.Instance.Base.transform.position, speed * currentSlow);
        transform.rotation = Quaternion.FromToRotation(transform.position, EnemyControllerScript.Instance.Base.transform.position);
	}
}
