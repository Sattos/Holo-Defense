using HoloToolkit.Examples.SpatialUnderstandingFeatureOverview;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int requestType = 8;

    public GameObject cube;// = Resources.Load<GameObject>("Prefabs/GameObject");

    public PlaceableObject curretObject;
    private GameObject objectToPlace;

    public enum ObjectsToPlace
    {
        cube
    }

    private PlaceableObject[] Objects;// = {new PlaceableObject(Resources.Load<GameObject>("Prefabs/GameObject"), 0.2f, 0.2f, 0.4f) };

    public void StartPlacingObject(ObjectsToPlace id)
    {
        //AppState.Instance.place = true;
        AppState.Instance.currentGameState = AppState.GameStates.PlaceObject;
        curretObject = Objects[(int)id];
        objectToPlace = Instantiate(curretObject.obj);
    }

    public void FinalizePlacement()
    {
        Debug.Log("PLACE");
        objectToPlace = null;
        AppState.Instance.currentGameState = AppState.GameStates.Game;
    }

    public void CancelPlacement()
    {
        Destroy(objectToPlace);
        AppState.Instance.currentGameState = AppState.GameStates.Game;
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
                quat = Quaternion.LookRotation(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)), testRes.normal);
                break;
            default:
                break;
        }

        //if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor)
        {
            Debug.Log(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)));
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
        }
        else
        {
            objectToPlace.SetActive(false);
        }

        return false;
    }

    // Use this for initialization
    void Start()
    {
        Objects = new PlaceableObject[] { new PlaceableObject(cube, 0.2f, 0.2f, 0.4f) };
        //Instantiate(Objects[0].obj);
    }

    // Update is called once per frame
    void Update()
    {

    }



}
