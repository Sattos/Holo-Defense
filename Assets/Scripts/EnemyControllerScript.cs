using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveParameters
{
    public float delay;
    public int count;
    public BaseEnemy.BaseStats stats;

    public WaveParameters(float delay, int count, BaseEnemy.BaseStats stats)
    {
        this.delay = delay;
        this.count = count;
        this.stats = stats;
    }

    public WaveParameters(float delay, int count, float health, float speed, int money)
    {
        this.delay = delay;
        this.count = count;
        this.stats = new BaseEnemy.BaseStats(health, speed, money);
    }
}

public class EnemyControllerScript : Singleton<EnemyControllerScript> {

    public BaseEnemy Enemy;
    public GameObject Base;

    public TextMesh waveText;

    public int baseHealth;

    public bool isBasePlaced = false;

    public bool isStarted = false;
    public int currentWave = 0;
    public int maxWave = 10;
    public WaveParameters[] waves =
        {   new WaveParameters(0.7f, 5, 10, 0.01f, 3),
            new WaveParameters(0.7f, 10, 7, 0.01f, 2),
            new WaveParameters(0.7f, 5, 20, 0.007f, 5),
            new WaveParameters(0.7f, 15, 10, 0.018f, 2),
            new WaveParameters(0.7f, 2, 60, 0.01f, 15),
            new WaveParameters(0.7f, 10, 25, 0.01f, 5),
            new WaveParameters(0.7f, 5, 10, 0.01f, 3),
            new WaveParameters(0.7f, 5, 10, 0.01f, 3),
            new WaveParameters(0.7f, 30, 25, 0.015f, 2),
            new WaveParameters(0.7f, 1, 300, 0.005f, 50),
            };

    public float waveTime;
    private float time;

    public enum EnemyType
    {
        Base,
        Flying
    }

    public enum TargetingMode
    {
        Farthest,
        Nearest,
        MaxHP,
        MinHP
    }

    List<BaseEnemy> Enemies = new List<BaseEnemy>();

    List<Spawner> Spawners = new List<Spawner>();

    public List<BaseEnemy> FindEnemiesInRange(Vector3 turretPosition, float range, int count, TargetingMode mode)
    {
        List<BaseEnemy> ret = new List<BaseEnemy>();
        List<BaseEnemy> list = new List<BaseEnemy>(Enemies);
        
        foreach (BaseEnemy obj in list)
        {
            if (Vector3.Distance(turretPosition, obj.transform.position) <= range)
            {
                ret.Add(obj);
            }
        }
        switch(mode)
        {
            case TargetingMode.Farthest:
                ret.Sort((x, y) => Vector3.Distance(x.transform.position, Base.transform.position).CompareTo(Vector3.Distance(y.transform.position, Base.transform.position)));
                break;
            case TargetingMode.Nearest:
                ret.Sort((x, y) => Vector3.Distance(x.transform.position, turretPosition).CompareTo(Vector3.Distance(y.transform.position, turretPosition)));
                break;
            case TargetingMode.MaxHP:
                ret.Sort((x, y) => y.stats.health.CompareTo(x.stats.health));
                break;
            case TargetingMode.MinHP:
                ret.Sort((x, y) => x.stats.health.CompareTo(y.stats.health));
                break;
        }
        
        ret = new List<BaseEnemy>(ret.GetRange(0, Mathf.Min(count, ret.Count)));
        return ret;
    }

    public void DestroyEnemy(BaseEnemy enemy)
    {
        Enemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }

    public void AddEnemy(BaseEnemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void AddSpawner(Spawner spawner)
    {
        Spawners.Add(spawner);
    }

    public void SendNextWave()
    {
        if (!isStarted || currentWave == maxWave)
        {
            return;
        }
        foreach(Spawner spawner in Spawners)
        {
            spawner.StartNextWave(waves[currentWave]);
        }
        currentWave++;
        time = Time.time + waveTime;
    }

    public void SendFirstWave()
    {
        if(!isBasePlaced)
        {
            AppState.Instance.Prompt("PLACE BASE FIRST", Color.red, 2.5f);
            return;
        }
        isStarted = true;
        time = Time.time + 2.0f;
        //SendNextWave();
    }

    public void SendWave()
    {
        if(isStarted)
        {
            SendNextWave();
        }
        else
        {
            SendFirstWave();
        }
    }

    public void DamageBase()
    {
        AppState.Instance.UpdateLivesText();
        if(--baseHealth <= 0)
        {
            //AppState.Instance.Restart();
            //AppState.Instance.SetUI(1);
            //GAME OVER
        }
    }

    // Use this for initialization
    void Start () {
        //Enemies = new List<GameObject>();
        //BaseEnemy o = Instantiate(Enemy);
        //o.transform.position = new Vector3(0, 0, 0);
        //Enemies.Add(o);

        //o = Instantiate(Enemy);
        //o.transform.position = new Vector3(0.5f, 0, 0.3f);
        //Enemies.Add(o);

        //o = Instantiate(Enemy);
        //o.transform.position = new Vector3(0, 0, 0.1f);
        //Enemies.Add(o);
    }
	
	// Update is called once per frame
	void Update () {
        if(currentWave == maxWave)
        {
            waveText.text = "";
            return;
        }
		if(isStarted)
        {
            float timeToNext = time - Time.time;

            waveText.text = String.Format("{0:0.0}s", timeToNext);
            if (timeToNext <= 0)
            {
                SendNextWave();
            }
        }
	}

    public void Restart()
    {
        foreach (Spawner spawner in Spawners)
        {
            if(spawner != null)
                Destroy(spawner.transform.parent.gameObject);
        }
        foreach (BaseEnemy enemy in Enemies)
        {
            if(enemy != null)
                DestroyEnemy(enemy);
        }
        Destroy(Base.gameObject);
        isBasePlaced = false;
        Spawners.Clear();
        Enemies.Clear();
    }
}
