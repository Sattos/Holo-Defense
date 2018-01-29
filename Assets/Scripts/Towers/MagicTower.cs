using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTower : BaseTower {

    public GameObject Missile;

    protected override void Attack()
    {
        GameObject o = Instantiate(Missile);
        o.transform.position = transform.position;
        Debug.Log("ATTACK");
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	//void Update () {
    //    base.Update();
	//}
}
