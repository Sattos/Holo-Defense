using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public EnemyControllerScript.EnemyType EnemyType;
    public BaseEnemy Enemy;

    public BaseEnemy.BaseStats Stats;

    public int count;
    public double delay;

    private int spawnedCount;
    private DateTime lastSpawn;

    public void SpawnEnemy(BaseEnemy.BaseStats stats)
    {
        BaseEnemy enemy = Instantiate(Enemy);
        enemy.stats = stats;
        enemy.health = stats.health;
        enemy.speed = stats.speed;
        enemy.money = stats.money;
        enemy.transform.position = transform.position;
        EnemyControllerScript.Instance.AddEnemy(enemy);
        if(++spawnedCount == count)
        {
            Destroy(this.gameObject);
        }
    }

	// Use this for initialization
	void Start () {
        Stats = new BaseEnemy.BaseStats(10, 0.01f, 5, false);
        spawnedCount = 0;
        lastSpawn = DateTime.MinValue;
	}
	
	// Update is called once per frame
	void Update () {
        if ((DateTime.Now - lastSpawn).TotalMilliseconds > delay)
        {
            SpawnEnemy(Stats);
            lastSpawn = DateTime.Now;
        }
    }
}
