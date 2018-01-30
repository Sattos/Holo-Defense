using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerScript : Singleton<EnemyControllerScript> {

    public BaseEnemy Enemy;
    public GameObject Base;

    public enum EnemyType
    {
        Base,
        Flying
    }

    private object lockObject = new object();

    List<BaseEnemy> Enemies = new List<BaseEnemy>();

    public BaseEnemy FindFarthestEnemyInRange(Vector3 turretPosition, float range)
    {
        BaseEnemy ret = null;
        float distanceFromBase = 0;
        lock (lockObject)
        {
            foreach (BaseEnemy obj in Enemies)
            {
                if (Vector3.Distance(turretPosition, obj.transform.position) > range)
                {
                    continue;
                }
                if (ret == null)
                {
                    ret = obj;
                    distanceFromBase = Vector3.Distance(ret.transform.position, Base.transform.position);
                    continue;
                }
                float newDistanceFromBase = Vector3.Distance(obj.transform.position, Base.transform.position);
                if (newDistanceFromBase < distanceFromBase)
                {
                    ret = obj;
                    distanceFromBase = newDistanceFromBase;
                }
            }
        }
        return ret;
    }

    public void DestroyEnemy(BaseEnemy enemy)
    {
        lock(lockObject)
        {
            Enemies.Remove(enemy);
        }
        Debug.Log(enemy.money);
        Destroy(enemy.gameObject);
    }

    public void AddEnemy(BaseEnemy enemy)
    {
        lock(lockObject)
        {
            Enemies.Add(enemy);
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
		
	}
}
