using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public BaseEnemy Target;

    public AttackStats stats;

    public Animation hitAnimation;

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
            if(stats.areaOfEffect > 0)
            {
                Debug.Log("boom");
                GameObject animParent = Instantiate(new GameObject());
                GameObject anim = Instantiate(hitAnimation.gameObject);
                animParent.transform.localScale = Vector3.one * stats.areaOfEffect;
                anim.transform.SetParent(animParent.transform);
                anim.transform.position = transform.position;
                anim.GetComponent<Animation>().Play();
                Destroy(animParent, anim.GetComponent<Animation>().clip.length);
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
            transform.position = Vector3.MoveTowards(transform.position, Target.GetTargetPosition().position, stats.velocity * Time.deltaTime);
            transform.eulerAngles = Vector3.RotateTowards(transform.position, EnemyControllerScript.Instance.Base.transform.position, 360, 0);
        }
	}
}
