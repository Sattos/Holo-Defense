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

    public GameObject StatsPanel;

    private BaseTower tower;

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
        tower.Upgrade();
        if (tower.IsMaxLevel())
        {
            upgradeButton.interactable = false;
        }
        ShowStats();
    }

    public void ShowStatsForTowerType(int type)
    {
        switch(type)
        {
            case 1:
                Values[0].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].damage);
                Values[1].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].attackSpeed);
                Values[2].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].range);
                Values[3].text = String.Format(FormatDOT, ProjectileTower.UpgradeLevels[0].damagePerSecond, ProjectileTower.UpgradeLevels[0].damageDuration);
                Values[4].text = String.Format(FormatSlow, ProjectileTower.UpgradeLevels[0].slow * 100, ProjectileTower.UpgradeLevels[0].slowDuration);
                Values[5].text = String.Format(FormatInteger, ProjectileTower.UpgradeLevels[0].targetCount);
                Values[6].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].radius);
                Values[7].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].velocity);
                StatsPanel.SetActive(true);
                break;
            case 2:
                Values[0].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].damage);
                Values[1].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].attackSpeed);
                Values[2].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].range);
                Values[3].text = String.Format(FormatDOT, RadiusTower.UpgradeLevels[0].damagePerSecond, RadiusTower.UpgradeLevels[0].damageDuration);
                Values[4].text = String.Format(FormatSlow, RadiusTower.UpgradeLevels[0].slow * 100, RadiusTower.UpgradeLevels[0].slowDuration);
                Values[5].text = "all in range"; //String.Format(FormatInteger, RadiusTower.UpgradeLevels[0].targetCount);
                Values[6].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].radius);
                Values[7].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].velocity);
                StatsPanel.SetActive(true);
                break;
            default:
                StatsPanel.SetActive(false);
                break;
        }
    }

    public void Activate(BaseTower tower)
    {
        if(this.tower == tower && this.isActiveAndEnabled)
        {
            Deactivate();
        }

        this.tower = tower;
        canvas.transform.position = tower.transform.position + tower.transform.rotation * new Vector3(0, 0.5f, 0);

        ShowStats();

        if(tower.IsMaxLevel())
        {
            upgradeButton.interactable = false;
        }
        else
        {
            upgradeButton.interactable = true;
        }

        canvas.gameObject.SetActive(true);
    }

    public void ShowStats()
    {
        Values[0].text = String.Format(FormatDecimal, tower.stats.damage);
        Values[1].text = String.Format(FormatDecimal, tower.attackSpeed);
        Values[2].text = String.Format(FormatDecimal, tower.range);
        Values[3].text = String.Format(FormatDOT, tower.stats.damagePerSecond, tower.stats.damageDuration);
        Values[4].text = String.Format(FormatSlow, tower.stats.slow * 100, tower.stats.slowDuration);
        Values[5].text = String.Format(FormatInteger, tower.targetCount);
        Values[6].text = String.Format(FormatDecimal, tower.stats.areaOfEffect);
        Values[7].text = String.Format(FormatDecimal, tower.stats.velocity);
    }

    public void ShowUpgradeStats()
    {
        if (tower == null)
            return;
        if(tower.IsMaxLevel())
        {
            return;
        }
        object upgradeStats = tower.GetNextUpgradeStats();

        ProjectileTower.UpgradeStats projectileUpgradeStats = upgradeStats as ProjectileTower.UpgradeStats;
        if(projectileUpgradeStats != null)
        {
            Values[0].text = String.Format(FormatUpdateDecimal, tower.stats.damage, projectileUpgradeStats.damage);
            Values[1].text = String.Format(FormatUpdateDecimal, tower.attackSpeed, projectileUpgradeStats.attackSpeed);
            Values[2].text = String.Format(FormatUpdateDecimal, tower.range, projectileUpgradeStats.range);
            Values[5].text = String.Format(FormatUpdateInteger, tower.targetCount, projectileUpgradeStats.targetCount);
            Values[7].text = String.Format(FormatUpdateDecimal, tower.stats.velocity, projectileUpgradeStats.velocity);
            return;
        }

        RadiusTower.UpgradeStats radiusUpgradeStats = upgradeStats as RadiusTower.UpgradeStats;
        if (radiusUpgradeStats != null)
        {
            Values[0].text = String.Format(FormatUpdateDecimal, tower.stats.damage, radiusUpgradeStats.damage);
            Values[1].text = String.Format(FormatUpdateDecimal, tower.attackSpeed, radiusUpgradeStats.attackSpeed);
            Values[2].text = String.Format(FormatUpdateDecimal, tower.range, radiusUpgradeStats.range);
            Values[4].text = String.Format(FormatUpdateSlow, tower.stats.slow * 100, tower.stats.slowDuration, radiusUpgradeStats.slow * 100, radiusUpgradeStats.slowDuration);
            return;
        }

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
        if (canvas == null)
            return;
        Vector3 lookDirTarget = CameraCache.Main.transform.position - canvas.transform.position;
        lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
        canvas.transform.rotation = Quaternion.Slerp(canvas.transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
    }
}
