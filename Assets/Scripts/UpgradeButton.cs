using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour, IRaycastFocusEvent {
    public void Activate()
    {
        TurretInfoCanvas.Instance.ShowUpgradeStats();
    }

    public void Deactivate()
    {
        TurretInfoCanvas.Instance.ShowStats();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
