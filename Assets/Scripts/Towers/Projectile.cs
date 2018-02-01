using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public BaseEnemy Target;

    public AttackStats stats;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetInstanceID() == Target.gameObject.GetInstanceID())
        {
            Target.Hit(stats);
            if(stats.areaOfEffect > 0)
            {
                Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, stats.areaOfEffect);
                foreach(Collider collider in enemiesInRange)
                {
                    BaseEnemy enemy = collider.GetComponent<BaseEnemy>();
                    if (enemy != null)
                        enemy.Hit(stats);
                }
            }
            Destroy(this.gameObject);
        }
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Target == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, stats.speed);
        }
	}
}
