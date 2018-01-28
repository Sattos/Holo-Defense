using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretInfoCanvas : MonoBehaviour {

    public void ClickTurret()
    {
        ObjectPlacer.Instance.TurretInfoCanvas.transform.position = transform.position + transform.rotation * new Vector3(0, 0.7f, 0);
        ObjectPlacer.Instance.TurretInfoCanvas.gameObject.SetActive(true);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
