using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ObjectPlacer.Instance.StartPlacingObject(ObjectPlacer.ObjectsToPlace.basePrefab);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
