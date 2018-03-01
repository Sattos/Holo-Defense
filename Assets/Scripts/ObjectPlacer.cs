using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPlacer : Singleton<ObjectPlacer>
{
    public struct PlaceableObject
    {
        public GameObject obj;
        public float halfWidth;
        public float halfLength;
        public float height;

        public PlaceableObject(GameObject obj, float width, float length, float height)
        {
            this.obj = obj;
            this.halfWidth = width/2.0f;
            this.halfLength = length/2.0f;
            this.height = height;
        }
    }

    private bool isValidLocation;

    private DateTime clickTime;

    public int requestType = 8;

    public GameObject archerTowerPrefab;// = Resources.Load<GameObject>("Prefabs/GameObject");
    public GameObject cannonTowerPrefab;
    public GameObject mageTowerPrefab;
    public GameObject basePrefab;
    public GameObject spawnerPrefab;

    public bool isNormalInterface;
    public GameObject ui;

    public bool isPlacing;

    public PlaceableObject curretObject;
    private GameObject objectToPlace;
    public ObjectsToPlace currentEnum { get; private set; }

    private int cost;

    //public enum ObjectsToPlace
    //{
    //    projectileTowerPrefab,
    //    radiusTowerPrefab,
    //    basePrefab,
    //    spawnerPrefab,
    //    cannonPrefab,
    //    none
    //}

    public enum ObjectsToPlace
    {
        archerTowerPrefab,
        cannonTowerPrefab,
        mageTowerPrefab,
        basePrefab,
        spawnerPrefab,
        none
    }

    private Dictionary<ObjectsToPlace, int> CostDictionary = new Dictionary<ObjectsToPlace, int>() {
                        {ObjectsToPlace.archerTowerPrefab, 10},
                        {ObjectsToPlace.cannonTowerPrefab, 10},
                        {ObjectsToPlace.mageTowerPrefab, 10},
    };

    private PlaceableObject[] Objects;// = {new PlaceableObject(Resources.Load<GameObject>("Prefabs/GameObject"), 0.2f, 0.2f, 0.4f) };

    public void StartPlacingObject(int obj)
    {
        StartPlacingObject((ObjectsToPlace)obj);
    }

    public void StartPlacingObject(ObjectsToPlace obj)
    {
        if(obj == ObjectsToPlace.none)
        {
            return;
        }
        if(isPlacing)
        {
            //CancelPlacement();
            Destroy(objectToPlace);
        }
        cost = 0;
        if(CostDictionary.TryGetValue(obj, out cost))
        {
            if(AppState.Instance.money < cost)
            {
                AppState.Instance.Prompt("Not enough gold", new Color(255, 0, 0, 255), 2.5f);
                return;
            }
            else
            {

            }
        }

        Debug.Log("PLACING");
        //if ((DateTime.Now - clickTime).Milliseconds < 50)
            //return;
        if(obj == ObjectsToPlace.basePrefab && EnemyControllerScript.Instance.isBasePlaced)
        {
            return;
        }
        //AppState.Instance.currentGameState = AppState.GameStates.PlaceObject;
        currentEnum = obj;
        curretObject = Objects[(int)obj];
        objectToPlace = Instantiate(curretObject.obj);
        SpatialUnderstandingCursor.Instance.CursorText.text = "start placing";
        clickTime = DateTime.Now;
        isPlacing = true;
    }

    public void FinishDragAndDrop()
    {
        if (!isValidLocation)
            CancelPlacement();
        else
        {
            switch (currentEnum)
            {
                case ObjectsToPlace.archerTowerPrefab:
                    (objectToPlace.GetComponentInChildren<ProjectileTower>()).FinalizePlacement();
                    break;
                case ObjectsToPlace.mageTowerPrefab:
                    (objectToPlace.GetComponentInChildren<RadiusTower>()).FinalizePlacement();
                    break;
                case ObjectsToPlace.basePrefab:
                    EnemyControllerScript.Instance.Base = objectToPlace.GetComponentInChildren<Transform>().gameObject;
                    EnemyControllerScript.Instance.isBasePlaced = true;

                    AppState.Instance.BadBaseButton.SetActive(false);
                    AppState.Instance.OptimalBaseButton.SetActive(false);

                    break;
                case ObjectsToPlace.spawnerPrefab:
                    EnemyControllerScript.Instance.AddSpawner(objectToPlace.GetComponentInChildren<Spawner>());
                    break;
                //case ObjectsToPlace.cannonPrefab:
                    //(objectToPlace.GetComponentInChildren<RadiusTower>()).FinalizePlacement();
                    //break;
            }

            objectToPlace = null;
            //AppState.Instance.currentGameState = AppState.GameStates.Game;
            SpatialUnderstandingCursor.Instance.CursorText.text = "finalize placing";
            isPlacing = false;
            AppState.Instance.money -= cost;
            AppState.Instance.UpdateMoneyText();
        }
    }

    public void FinalizePlacement()
    {
        //if ((DateTime.Now - clickTime).Milliseconds < 50)
            //return;
        if (!isValidLocation)
            return;
        Debug.Log("PLACE");

        switch (currentEnum)
        {
            case ObjectsToPlace.archerTowerPrefab:
            case ObjectsToPlace.cannonTowerPrefab:
                (objectToPlace.GetComponentInChildren<ProjectileTower>()).FinalizePlacement();
                break;
            case ObjectsToPlace.mageTowerPrefab:
                (objectToPlace.GetComponentInChildren<RadiusTower>()).FinalizePlacement();
                break;
            case ObjectsToPlace.basePrefab:
                EnemyControllerScript.Instance.Base = objectToPlace.GetComponentInChildren<Transform>().gameObject;
                if(isNormalInterface)
                {
                    ui.transform.position = objectToPlace.transform.position + objectToPlace.transform.rotation * new Vector3(0, 0.7f, 0);
                    ui.SetActive(true);
                    ui.GetComponent<NormalUI>().enabled = true;
                }

                AppState.Instance.BadBaseButton.SetActive(false);
                AppState.Instance.OptimalBaseButton.SetActive(false);

                EnemyControllerScript.Instance.isBasePlaced = true;
                break;
            case ObjectsToPlace.spawnerPrefab:
                EnemyControllerScript.Instance.AddSpawner(objectToPlace.GetComponentInChildren<Spawner>());
                break;
            //case ObjectsToPlace.cannonPrefab:
                //(objectToPlace.GetComponentInChildren<RadiusTower>()).FinalizePlacement();
                //break;
        }

        objectToPlace = null;
        //AppState.Instance.currentGameState = AppState.GameStates.Game;
        SpatialUnderstandingCursor.Instance.CursorText.text = "finalize placing";
        clickTime = DateTime.Now;
        isPlacing = false;
        AppState.Instance.money -= cost;
        AppState.Instance.UpdateMoneyText();
    }

    public void CancelPlacement()
    {
        if (objectToPlace != null)
        {
            //if ((DateTime.Now - clickTime).Milliseconds < 20)
                //return;
            DestroyImmediate(objectToPlace);
            objectToPlace = null;
            //AppState.Instance.currentGameState = AppState.GameStates.Game;
            SpatialUnderstandingCursor.Instance.CursorText.text = "cancel placing";
            clickTime = DateTime.Now;
            isPlacing = false;
            Debug.Log("CancelPlacement");
        }
    }

    public float refreshRate = 0.0f;
    private float nextRefresh = 0.0f;

    private SpatialUnderstandingDllTopology.TopologyResult testRes;
    private int testResPtr;

    public bool CheckPlacement(SpatialUnderstandingDll.Imports.RaycastResult rayCastResult)
    {
        while (Time.time < nextRefresh)
        {
            //yield null;
        }
        nextRefresh = Time.time + refreshRate;

        IntPtr test = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(testRes);

        testRes.normal = rayCastResult.IntersectNormal;
        testRes.position = rayCastResult.IntersectPoint;
        testRes.length = curretObject.halfLength;
        testRes.width = curretObject.halfWidth;

        Quaternion quat = Quaternion.LookRotation(CameraCache.Main.transform.forward, testRes.normal);


        switch ((int)rayCastResult.SurfaceType)
        {
            case 0://invalid
                break;
            case 1://other
                break;
            case 2://floor
            case 3://floorlike
            case 4://platform
                quat = Quaternion.LookRotation(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)), testRes.normal);
                break;
            case 5://ceiling
                quat = Quaternion.LookRotation(Vector3.Scale(CameraCache.Main.transform.forward, (-Vector3.one - testRes.normal)), testRes.normal);
                break;
            case 6://wallexternal
            case 7://walllike
                Vector3 v = CameraCache.Main.transform.forward;
                Vector3.OrthoNormalize(ref testRes.normal, ref v);
                quat = Quaternion.LookRotation(v, testRes.normal);

                SpatialUnderstandingCursor.Instance.CursorText.text = String.Format("x={0}, y={1}, z={2}\nx={3}, y={4}, z={5}", testRes.normal.x, testRes.normal.y, testRes.normal.z, CameraCache.Main.transform.rotation.eulerAngles.x/360, CameraCache.Main.transform.rotation.eulerAngles.y/360, CameraCache.Main.transform.rotation.eulerAngles.z/360);
                break;
            default:
                break;
        }

        //if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor)
        {
            //Debug.Log(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)));
            //return;
        }

        Vector3 topLeft = testRes.position + quat * new Vector3(-curretObject.halfLength, 0f, -curretObject.halfWidth);
        Vector3 topRight = testRes.position + quat * new Vector3(-curretObject.halfLength, 0f, curretObject.halfWidth);
        Vector3 bottomLeft = testRes.position + quat * new Vector3(curretObject.halfLength, 0f, -curretObject.halfWidth);
        Vector3 bottomRight = testRes.position + quat * new Vector3(curretObject.halfLength, 0f, curretObject.halfWidth);

        //Debug.Log("ray:" + testRes.position + "\nnorm" + testRes.normal + "\nTL:" + topLeft + "\nTR:" + topRight + "\nBR:" + bottomRight + "\nBL:" + bottomLeft);

        bool isValidRect = SpatialUnderstandingDllTopology.QueryTopology_IsValidRect(topLeft, topRight, bottomLeft, bottomRight, requestType, (int)rayCastResult.SurfaceType, test);

        if (isValidRect)
        {
            objectToPlace.transform.SetPositionAndRotation(testRes.position, quat);
            objectToPlace.SetActive(true);
            //objectToPlace.GetComponentInChildren<Collision>().testGreen();
            AppState.Instance.DebugDisplay.text = "OK";
            isValidLocation = true;
        }
        else
        {
            objectToPlace.transform.SetPositionAndRotation(testRes.position, quat);
            AppState.Instance.DebugDisplay.text = "Bad location";
            //objectToPlace.GetComponentInChildren<Collision>().testRed();
            //objectToPlace.SetActive(false);
            isValidLocation = false;
        }
        
        return false;
    }

    // Use this for initialization
    void Start()
    {
        Objects = new PlaceableObject[] {
            new PlaceableObject(archerTowerPrefab, 0.2f, 0.2f, 0.4f),
            new PlaceableObject(cannonTowerPrefab, 0.2f, 0.2f, 0.4f),
            new PlaceableObject(mageTowerPrefab, 0.2f, 0.2f, 0.4f),
            new PlaceableObject(basePrefab, 0.5f, 0.5f, 0.5f),
            new PlaceableObject(spawnerPrefab, 0.3f, 0.3f, 0.3f),
        };
        isValidLocation = false;
        clickTime = DateTime.MinValue;
        //Instantiate(Objects[0].obj);
    }

    // Update is called once per frame
    void Update()
    {

    }



}
