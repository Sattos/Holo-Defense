using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableClick : MonoBehaviour, IRaycastFocusEvent {

    public GameObject RotatingObject;

    public TurretInfoCanvas TurretInfoCanvas;

    public ObjectPlacer.ObjectsToPlace towerType;

    public bool isRotating;

    public bool BlockPlacement
    {
        get
        {
            return true;
        }

        set
        {
            
        }
    }

    public ObjectPlacer.ObjectsToPlace BlockingType
    {
        get
        {
            return towerType;
        }

        set
        {
            
        }
    }

    public void Activate()
    {
        isRotating = true;
        if ((int)towerType == 1 || (int)towerType == 2) //TODO for all turrets
        {
            if (TurretInfoCanvas != null)
            {
                TurretInfoCanvas.ShowStatsForTowerType(towerType);
            }
            //TurretInfoCanvas.gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        isRotating = false;
        if (TurretInfoCanvas != null)
        {
            TurretInfoCanvas.gameObject.SetActive(false);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isRotating)
        {
            RotatingObject.transform.Rotate(0, 1, 0);
        }
	}
}
