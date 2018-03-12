﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public EnemyControllerScript.EnemyType EnemyType;
    public BaseEnemy Enemy;

    public BaseEnemy.BaseStats stats;

    public int count;
    public double delay;

    private int spawnedCount;
    private DateTime lastSpawn;

    public void SpawnEnemy(BaseEnemy.BaseStats stats)
    {
        BaseEnemy enemy = Instantiate(Enemy);
        enemy.GetComponent<playerControl>().FlyForward();
        enemy.stats = stats;
        enemy.health = stats.health;
        enemy.speed = stats.speed;
        enemy.money = stats.money;
        enemy.transform.position = transform.position;

        Vector3 _direction = (EnemyControllerScript.Instance.Base.transform.position - transform.position).normalized;

        //create the rotation we need to be in to look at the target
        Quaternion _lookRotation = Quaternion.LookRotation(_direction);

        enemy.transform.rotation = _lookRotation;
        enemy.transform.eulerAngles += new Vector3(0, 90, 0);

        EnemyControllerScript.Instance.AddEnemy(enemy);
        if(++spawnedCount == count)
        {
            Destroy(this.gameObject);
        }
    }

    public void StartNextWave(WaveParameters waveParameters)
    {
        StartCoroutine(StartWave(waveParameters.delay, waveParameters.count, waveParameters.stats));
    }

    private IEnumerator StartWave(float delay, int count, BaseEnemy.BaseStats stats)
    {
        //while(true)
        for(int i = 0; i < count; i++)
        {
            SpawnEnemy(stats);
            yield return new WaitForSeconds(delay);
        }
    }

	// Use this for initialization
	void Start () {
        //stats = new BaseEnemy.BaseStats(10, 0.01f, 5, false);
        spawnedCount = 0;
        lastSpawn = DateTime.MinValue;
	}
	
	// Update is called once per frame
	void Update () {
        if ((DateTime.Now - lastSpawn).TotalMilliseconds > delay)
        {
            //SpawnEnemy(Stats);
            lastSpawn = DateTime.Now;
        }
    }
}
