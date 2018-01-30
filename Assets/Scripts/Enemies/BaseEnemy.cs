using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour {

    public float health;
    public float speed;
    public float money;
    public BaseStats stats;

    public struct BaseStats
    {
        public float health;
        public float speed;
        public float money;
        public bool isFlying;

        public BaseStats(float health, float speed, float money, bool isFlying)
        {
            this.health = health;
            this.speed = speed;
            this.money = money;
            this.isFlying = isFlying;
        }
    }

    public void Damage(float value)
    {
        health -= value;
        if (health < 0)
        {
            EnemyControllerScript.Instance.DestroyEnemy(this);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.MoveTowards(transform.position, EnemyControllerScript.Instance.Base.transform.position, speed);
	}
}
