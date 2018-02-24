using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour, IRaycastFocusEvent {

    public bool BlockPlacement
    {
        get
        {
            return false;
        }

        set
        {

        }
    }

    public ObjectPlacer.ObjectsToPlace BlockingType
    {
        get
        {
            return ObjectPlacer.ObjectsToPlace.none;
        }

        set
        {
            
        }
    }

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
