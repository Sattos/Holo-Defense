using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour, IRaycastFocusEvent {
    public void Activate()
    {
        AppState.Instance.TurretInfoCanvas.ShowUpgradeStats();
    }

    public void Deactivate()
    {
        AppState.Instance.TurretInfoCanvas.ShowStats();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
