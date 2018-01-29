using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerScript : Singleton<EnemyControllerScript> {

    public GameObject Enemy;

    List<GameObject> Enemies = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //Enemies = new List<GameObject>();
        GameObject o = Instantiate(Enemy);
        Enemies.Add(o);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
