﻿using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretInfoCanvas : Singleton<TurretInfoCanvas> {

    public Canvas canvas;

    public Text[] Values;

    private BaseTower tower;

    enum Properties
    {
        Damage,
        Speed,
        Range,
        DOT,
        Slow,
        Targets,
        Radius,
        Flying
    }



    static string FormatUpdateDecimal = "{0:0.00}->{1:0.00}";
    static string FormatUpdateSlow = "{0}%/{1:0.0}s->{2}%/{3:0.0}s";
    static string FormatUpdateDOT = "{0:0.0}/{1:0.0}s->{2:0.0}/{3:0.0}s";
    static string FormatUpdateInteger = "{0}->{1}";

    static string FormatDecimal = "{0:0.00}";
    static string FormatSlow = "{0}%/{1:0.0}s";
    static string FormatDOT = "{0:0.0}/{1:0.0}s";
    static string FormatInteger = "{0}";

    public void Activate(BaseTower tower)
    {
        this.tower = tower;
        canvas.transform.position = tower.transform.position + tower.transform.rotation * new Vector3(0, 0.5f, 0);

        ShowStats();

        canvas.gameObject.SetActive(true);
    }

    public void ShowStats()
    {
        Values[0].text = String.Format(FormatDecimal, tower.stats.damage);
        Values[1].text = String.Format(FormatDecimal, tower.stats.speed);
        Values[2].text = String.Format(FormatDecimal, tower.range);
        Values[3].text = String.Format(FormatDOT, tower.stats.damagePerSecond, tower.stats.damageDuration);
        Values[4].text = String.Format(FormatSlow, tower.stats.slow * 100, tower.stats.slowDuration);
        Values[5].text = String.Format(FormatInteger, tower.targetCount);
        Values[6].text = String.Format(FormatDecimal, tower.range);
        Values[7].text = tower.targetFlying.ToString();
    }

    public void ShowUpdateStats()
    {
        Values[0].text = String.Format(FormatUpdateDecimal, tower.stats.damage, tower.stats.damage * 1.2f);
        Values[1].text = String.Format(FormatUpdateDecimal, tower.stats.speed, tower.stats.speed * 1.2f);
        Values[2].text = String.Format(FormatUpdateDecimal, tower.range, tower.range * 1.2f);
        Values[3].text = String.Format(FormatUpdateDOT, tower.stats.damagePerSecond, tower.stats.damageDuration, tower.stats.damagePerSecond * 1.2f, tower.stats.damageDuration);
        Values[4].text = String.Format(FormatUpdateSlow, tower.stats.slow * 100, tower.stats.slowDuration, tower.stats.slow * 100 * 1.2f, tower.stats.slowDuration * 1.2f);
        Values[5].text = String.Format(FormatUpdateInteger, tower.targetCount, tower.targetCount + 1);
        Values[6].text = String.Format(FormatUpdateDecimal, tower.range, tower.range * 1.2f);
        Values[7].text = tower.targetFlying.ToString();
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
        Vector3 lookDirTarget = CameraCache.Main.transform.position - canvas.transform.position;
        lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
    }
}
