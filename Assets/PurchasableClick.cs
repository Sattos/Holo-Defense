using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchasableClick : MonoBehaviour, IRaycastFocusEvent {

    public GameObject RotatingObject;

    public TurretInfoCanvas TurretInfoCanvas;

    public int towerType;

    public bool isRotating;

    public void Activate()
    {
        isRotating = true;
        if (towerType == 1 || towerType == 2)
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
