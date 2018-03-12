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
    public float nextWaveTime;

    public WaveParameters(float delay, int count, BaseEnemy.BaseStats stats)
    {
        this.delay = delay;
        this.count = count;
        this.stats = stats;
    }

    public WaveParameters(float delay, int count, float health, float speed, int money, float nextWaveTime)
    {
        this.delay = delay;
        this.count = count;
        this.stats = new BaseEnemy.BaseStats(health, speed, money);
        this.nextWaveTime = nextWaveTime;
    }
}

public class EnemyControllerScript : Singleton<EnemyControllerScript> {

    public BaseEnemy Enemy;
    public GameObject Base;

    public TextMesh waveText;

    public Text badStartText;
    public Text normalStartText;
    public Text goodStartText;

    public int baseHealth;

    public bool isBasePlaced = false;

    public bool isStarted = false;
    public int currentWave = 0;
    public int maxWave = 10;
    public WaveParameters[] waves =
        {   new WaveParameters(0.7f, 5, 10, 0.3f, 3, 20),
            new WaveParameters(0.7f, 10, 7, 0.3f, 2, 20),
            new WaveParameters(0.7f, 5, 20, 0.15f, 5, 20),
            new WaveParameters(0.7f, 15, 10, 0.5f, 2, 20),
            new WaveParameters(0.7f, 2, 60, 0.3f, 15, 20),
            new WaveParameters(0.7f, 10, 25, 0.3f, 5, 20),
            new WaveParameters(0.7f, 5, 10, 0.3f, 3, 20),
            new WaveParameters(0.7f, 5, 10, 0.3f, 3, 20),
            new WaveParameters(0.7f, 30, 25, 0.6f, 2, 30),
            new WaveParameters(0.7f, 1, 300, 0.1f, 50, 0),
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
        if(currentWave == maxWave && Enemies.Count == 0)
        {
            AppState.Instance.Prompt("YOU WIN", Color.red, 5.0f);
        }
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
        time = waves[currentWave].nextWaveTime;
        currentWave++;
    }

    public void SendFirstWave()
    {
        if(!isBasePlaced)
        {
            AppState.Instance.Prompt("PLACE BASE FIRST", Color.red, 2.5f);
            return;
        }
        if(Spawners.Count == 0)
        {
            AppState.Instance.Prompt("PLACE SPAWNER FIRST", Color.red, 2.5f);
            return;
        }
        isStarted = true;
        time = 2.0f;//Time.time + 2.0f;
        badStartText.text = "Next wave";
        normalStartText.text = "Next wave";
        goodStartText.text = "Next wave";

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
            Time.timeScale = 0;
            AppState.Instance.Prompt("YOU LOSE\nPRESS MENU TO EXIT", Color.red, 5.0f);
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
            waveText.text = "END";
            badStartText.text = "END";
            normalStartText.text = "END";
            goodStartText.text = "END";
            return;
        }
		if(isStarted)
        {
            //float timeToNext = time - Time.time;
            time -= Time.deltaTime;

            waveText.text = String.Format("{0:0.0}s", time);
            if (time <= 0)
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
                Destroy(enemy.gameObject);
        }
        Destroy(Base.gameObject);
        isBasePlaced = false;
        Spawners.Clear();
        Enemies.Clear();
        badStartText.text = "Start";
        normalStartText.text = "Start";
        goodStartText.text = "Start";
    }
}
