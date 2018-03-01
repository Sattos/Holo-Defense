﻿using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class TurretInfoCanvas : Singleton<TurretInfoCanvas> {
public class TurretInfoCanvas : MonoBehaviour {

    public Canvas canvas;

    public Text[] Values;

    public Button upgradeButton;
    public Text upgradeText;

    public GameObject StatsPanel;

    private BaseTower tower;

    public bool dontRotate;

    public enum Properties
    {
        Damage,
        AttackSpeed,
        Range,
        DOT,
        Slow,
        Targets,
        Radius,
        Velocity
    }



    static string FormatUpdateDecimal = "{0:0.00}->{1:0.00}";
    static string FormatUpdateSlow = "{0}%/{1:0.0}s->{2}%/{3:0.0}s";
    static string FormatUpdateDOT = "{0:0.0}/{1:0.0}s->{2:0.0}/{3:0.0}s";
    static string FormatUpdateInteger = "{0}->{1}";

    static string FormatDecimal = "{0:0.00}";
    static string FormatSlow = "{0}%/{1:0.0}s";
    static string FormatDOT = "{0:0.0}/{1:0.0}s";
    static string FormatInteger = "{0}";

    public void Upgrade()
    {
        if(tower.UpgradeCost > AppState.Instance.money)
        {
            AppState.Instance.Prompt("Not enough gold", new Color(255, 0, 0, 255), 2.5f);
            return;
        }
        AppState.Instance.money -= tower.UpgradeCost;
        AppState.Instance.UpdateMoneyText();

        tower.Upgrade();
        if (tower.IsMaxLevel())
        {
            upgradeButton.interactable = false;
        }
        ShowStats(tower.towerType, tower.level);
    }

    public void Sell()
    {
        AppState.Instance.money += tower.SellValue;
        AppState.Instance.UpdateMoneyText();
        Destroy(tower.gameObject);
    }

    public void Activate(BaseTower tower)
    {
        if(this.tower == tower && this.isActiveAndEnabled)
        {
            Deactivate();
            return;
        }

        this.tower = tower;
        canvas.transform.position = tower.transform.position + tower.transform.rotation * new Vector3(0, 1.0f, 0);

        ShowStats(tower.towerType, tower.level);

        canvas.gameObject.SetActive(true);
    }

    public void ShowStats(TowerType type, int level, bool upgrade = false)
    {
        StatsPanel.GetComponent<InfoPanel>().ShowStats(type, level, upgrade);
    }

    public void ShowUpdateStats()
    {
        StatsPanel.GetComponent<InfoPanel>().ShowStats(tower.towerType, tower.level, true);
    }

    public void ShowStats()
    {
        StatsPanel.GetComponent<InfoPanel>().ShowStats(tower.towerType, tower.level);
    }

    public void Deactivate()
    {
        canvas.gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        Deactivate();
	}
	
	// Update is called once per frame
	void Update () {
        if (canvas == null || dontRotate)
            return;
        Vector3 lookDirTarget = CameraCache.Main.transform.position - canvas.transform.position;
        lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
    }
}
