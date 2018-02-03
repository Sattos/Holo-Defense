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

    public enum TargetingMode
    {
        Farthest,
        Nearest,
        MaxHP,
        MinHP
    }

    List<BaseEnemy> Enemies = new List<BaseEnemy>();

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
        Debug.Log(enemy.money);
        Destroy(enemy.gameObject);
    }

    public void AddEnemy(BaseEnemy enemy)
    {
        Enemies.Add(enemy);
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
