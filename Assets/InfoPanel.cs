using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

    public Text[] Values;

    static string FormatUpdateDecimal = "{0:0.00}->{1:0.00}";
    static string FormatUpdateSlow = "{0}%/{1:0.0}s->{2}%/{3:0.0}s";
    static string FormatUpdateDOT = "{0:0.0}/{1:0.0}s->{2:0.0}/{3:0.0}s";
    static string FormatUpdateInteger = "{0}->{1}";

    static string FormatDecimal = "{0:0.00}";
    static string FormatSlow = "{0}%/{1:0.0}s";
    static string FormatDOT = "{0:0.0}/{1:0.0}s";
    static string FormatInteger = "{0}";

    //public void ShowStatsForTowerType(ObjectPlacer.ObjectsToPlace type)
    //{
    //    switch (type)
    //    {
    //        case ObjectPlacer.ObjectsToPlace.archerTowerPrefab:
    //            Values[0].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].damage);
    //            Values[1].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].attackSpeed);
    //            Values[2].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].range);
    //            Values[3].text = String.Format(FormatDOT, ProjectileTower.UpgradeLevels[0].damagePerSecond, ProjectileTower.UpgradeLevels[0].damageDuration);
    //            Values[4].text = String.Format(FormatSlow, ProjectileTower.UpgradeLevels[0].slow * 100, ProjectileTower.UpgradeLevels[0].slowDuration);
    //            Values[5].text = String.Format(FormatInteger, ProjectileTower.UpgradeLevels[0].targetCount);
    //            Values[6].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].radius);
    //            Values[7].text = String.Format(FormatDecimal, ProjectileTower.UpgradeLevels[0].velocity);
    //            gameObject.SetActive(true);
    //            break;
    //        case ObjectPlacer.ObjectsToPlace.mageTowerPrefab:
    //            Values[0].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].damage);
    //            Values[1].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].attackSpeed);
    //            Values[2].text = String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].range);
    //            Values[3].text = String.Format(FormatDOT, RadiusTower.UpgradeLevels[0].damagePerSecond, RadiusTower.UpgradeLevels[0].damageDuration);
    //            Values[4].text = String.Format(FormatSlow, RadiusTower.UpgradeLevels[0].slow * 100, RadiusTower.UpgradeLevels[0].slowDuration);
    //            Values[5].text = "all in range"; //String.Format(FormatInteger, RadiusTower.UpgradeLevels[0].targetCount);
    //            Values[6].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].radius);
    //            Values[7].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].velocity);
    //            gameObject.SetActive(true);
    //            break;
    //        default:
    //            gameObject.SetActive(false);
    //            break;
    //    }
    //}

    public void ShowStats(TowerType type, int level, bool upgrade = false)
    {
        if (!upgrade || level == Stats.UpgradeLevels[type].Length - 1)
        {
            gameObject.SetActive(true);
            switch (type)
            {
                case TowerType.Archer:
                case TowerType.Cannon:
                    Values[0].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].damage);
                    Values[1].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].attackSpeed);
                    Values[2].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].range);
                    Values[3].text = String.Format(FormatDOT, Stats.UpgradeLevels[type][level].damagePerSecond, Stats.UpgradeLevels[type][level].damageDuration);
                    Values[4].text = String.Format(FormatSlow, Stats.UpgradeLevels[type][level].slow * 100, Stats.UpgradeLevels[type][level].slowDuration);
                    Values[5].text = String.Format(FormatInteger, Stats.UpgradeLevels[type][level].targetCount);
                    Values[6].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].radius);
                    Values[7].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].velocity);
                    break;
                case TowerType.Mage:
                    Values[0].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].damage);
                    Values[1].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].attackSpeed);
                    Values[2].text = String.Format(FormatDecimal, Stats.UpgradeLevels[type][level].range);
                    Values[3].text = String.Format(FormatDOT, Stats.UpgradeLevels[type][level].damagePerSecond, Stats.UpgradeLevels[type][level].damageDuration);
                    Values[4].text = String.Format(FormatSlow, Stats.UpgradeLevels[type][level].slow * 100, Stats.UpgradeLevels[type][level].slowDuration);
                    Values[5].text = "all in range"; //String.Format(FormatInteger, RadiusTower.UpgradeLevels[0].targetCount);
                    Values[6].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].radius);
                    Values[7].text = "-";//String.Format(FormatDecimal, RadiusTower.UpgradeLevels[0].velocity);
                    break;
                default:
                    gameObject.SetActive(false);
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case TowerType.Archer:
                case TowerType.Cannon:
                    Values[0].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].damage, Stats.UpgradeLevels[type][level + 1].damage);
                    Values[1].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].attackSpeed, Stats.UpgradeLevels[type][level + 1].attackSpeed);
                    Values[2].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].range, Stats.UpgradeLevels[type][level + 1].range);
                    Values[5].text = String.Format(FormatUpdateInteger, Stats.UpgradeLevels[type][level].targetCount, Stats.UpgradeLevels[type][level + 1].targetCount);
                    Values[7].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].velocity, Stats.UpgradeLevels[type][level + 1].velocity);
                    break;

                case TowerType.Mage:
                    Values[0].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].damage, Stats.UpgradeLevels[type][level].damage);
                    Values[1].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].attackSpeed, Stats.UpgradeLevels[type][level].attackSpeed);
                    Values[2].text = String.Format(FormatUpdateDecimal, Stats.UpgradeLevels[type][level].range, Stats.UpgradeLevels[type][level].range);
                    Values[4].text = String.Format(FormatUpdateSlow, Stats.UpgradeLevels[type][level].slow * 100, Stats.UpgradeLevels[type][level], Stats.UpgradeLevels[type][level].slow * 100, Stats.UpgradeLevels[type][level].slowDuration);
                    break;
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
