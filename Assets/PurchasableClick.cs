using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PurchasableClick : RaycastButton {

    public GameObject RotatingObject;

    //public TurretInfoCanvas TurretInfoCanvas;
    public InfoPanel InfoPanel;

    public ObjectPlacer.ObjectsToPlace towerType;

    public bool isRotating;

    private RectTransform rectTransform;

    public void ActivateRotation()
    {
        isRotating = true;
        if (towerType < ObjectPlacer.ObjectsToPlace.basePrefab) //TODO for all turrets
        {
            if (InfoPanel != null)
            {
                //InfoPanel.ShowStatsForTowerType(towerType);
                InfoPanel.ShowStats((TowerType)towerType, 0);
                if (AppState.Instance.currentGameState == AppState.GameStates.GoodInterface)
                {
                    if (gameObject.GetComponent<RectTransform>().anchoredPosition.x > 200.0f)
                    {
                        if (rectTransform.anchoredPosition.x > -100.0f)
                        {
                            rectTransform.anchoredPosition += new Vector2(-200.0f, 0);
                            GoodUI.Instance.ActivateRight();
                            GoodUI.Instance.DeactivateLeft();
                        }
                    }
                    else
                    {
                        if (rectTransform.anchoredPosition.x < -100.0f)
                        {
                            rectTransform.anchoredPosition += new Vector2(200.0f, 0);
                            GoodUI.Instance.ActivateLeft();
                            GoodUI.Instance.DeactivateRight();
                        }
                    }
                }
                else if (AppState.Instance.currentGameState == AppState.GameStates.NormalInterface)
                {
                    rectTransform.anchoredPosition = new Vector3(gameObject.GetComponent<RectTransform>().anchoredPosition.x, 100);
                }
            }
            //TurretInfoCanvas.gameObject.SetActive(true);
        }
    }

    public void DeactivateRotation()
    {
        isRotating = false;
        if (InfoPanel != null)
        {
            InfoPanel.gameObject.SetActive(false);
            GoodUI.Instance.ActivateLeft();
            GoodUI.Instance.ActivateRight();
        }
    }

    // Use this for initialization
    void Start () {
        OnFocusEntered.AddListener(ActivateRotation);
        OnFocusLost.AddListener(DeactivateRotation);

        if (InfoPanel != null)
        {
            rectTransform = InfoPanel.GetComponent<RectTransform>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(isRotating && RotatingObject != null)
        {
            RotatingObject.transform.Rotate(0, 60 * Time.deltaTime, 0);
        }
	}
}
