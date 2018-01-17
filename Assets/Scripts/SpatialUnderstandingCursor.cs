// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using HoloToolkit.Unity;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Button = UnityEngine.UI.Button;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    public class SpatialUnderstandingCursor : SpatialUnderstandingBasicCursor
    {
        public GameObject cube;
        public GameObject cubePosition;

        public static SpatialUnderstandingCursor Instance;

        override protected void Awake()
        {
            base.Awake();
            Instance = this;
        }

        // Consts
        public const float RayCastLength = 10.0f;

        // Config
        public TextMesh CursorText;
        public LayerMask UILayerMask;

        // Privates
        private SpatialUnderstandingDll.Imports.RaycastResult rayCastResult;

        const int QueryResultMaxCount = 10240;
        private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopologyFloor = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];
        private int locationCountWall;
        private SpatialUnderstandingDllTopology.TopologyResult[] resultsTopologyWall = new SpatialUnderstandingDllTopology.TopologyResult[QueryResultMaxCount];
        private int locationCountFloor;

        private SpatialUnderstandingDllTopology.TopologyResult testRes;
        private int testResPtr;

        public int requestType = 8;


        private float halfHeight;
        private float halfWidth;
        private float minHeight;
        private float halfLength;

        public void CreateTopology(float minHeightOfWallSpace, float minWidthOfWallSpace, float minHeightAboveFloor, float minFacingClearance)
        {
            //Wall
            IntPtr resultsTopologyWallPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopologyWall);
            locationCountWall = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnWalls(
                minHeightOfWallSpace, minWidthOfWallSpace, minHeightAboveFloor, minFacingClearance,
                resultsTopologyWall.Length, resultsTopologyWallPtr);
            Debug.Log("Wall" + locationCountWall);
            //Floor
            IntPtr resultsTopologyFloorPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopologyFloor);
            locationCountFloor = SpatialUnderstandingDllTopology.QueryTopology_FindPositionsOnFloor(
                minWidthOfWallSpace, minHeightOfWallSpace,
                resultsTopologyFloor.Length, resultsTopologyFloorPtr);
            Debug.Log("Floor" + locationCountFloor);

            halfHeight = minHeightOfWallSpace / 2.0f;
            halfWidth = minWidthOfWallSpace / 2.0f;
            minHeight = minHeightAboveFloor;
            halfLength = 0.2f;

            for(int i =0; i<locationCountWall;i++)
            {
                //Debug.Log(resultsTopologyFloor[i].normal);
            }
        }

        // Functions
        protected override RaycastResult CalculateRayIntersect()
        {
            RaycastResult result = base.CalculateRayIntersect();

            // Now use the understanding code
            if (SpatialUnderstanding.Instance.AllowSpatialUnderstanding &&
                SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done)
            {
                Vector3 rayPos = CameraCache.Main.transform.position;
                Vector3 rayVec = CameraCache.Main.transform.forward * RayCastLength;
                IntPtr raycastResultPtr = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResultPtr();
                SpatialUnderstandingDll.Imports.PlayspaceRaycast(
                    rayPos.x, rayPos.y, rayPos.z,
                    rayVec.x, rayVec.y, rayVec.z,
                    raycastResultPtr);
                rayCastResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticRaycastResult();

                float rayCastResultDist = Vector3.Distance(rayPos, rayCastResult.IntersectPoint);
                float resultDist = Vector3.Distance(rayPos, result.Position);

                // Override
                if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid &&
                    rayCastResultDist < resultDist)
                {
                    result.Hit = true;
                    result.Position = rayCastResult.IntersectPoint;
                    result.Normal = rayCastResult.IntersectNormal;

                    return result;
                }
            }

            return result;
        }

        public bool RayCastUI(out Vector3 hitPos, out Vector3 hitNormal, out Button hitButton)
        {
            // Defaults
            hitPos = Vector3.zero;
            hitNormal = Vector3.zero;
            hitButton = null;

            // Do the raycast
            RaycastHit hitInfo;
            Vector3 uiRayCastOrigin = CameraCache.Main.transform.position;
            Vector3 uiRayCastDirection = CameraCache.Main.transform.forward;
            if (Physics.Raycast(uiRayCastOrigin, uiRayCastDirection, out hitInfo, RayCastLength, UILayerMask))
            {
                Canvas canvas = hitInfo.collider.gameObject.GetComponent<Canvas>();
                if (canvas != null)
                {
                    GraphicRaycaster canvasRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();
                    if (canvasRaycaster != null)
                    {
                        // Cast only against this canvas
                        PointerEventData pData = new PointerEventData(EventSystem.current);

                        pData.position = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                        pData.delta = Vector2.zero;
                        pData.scrollDelta = Vector2.zero;

                        List<UnityEngine.EventSystems.RaycastResult> canvasHits = new List<UnityEngine.EventSystems.RaycastResult>();
                        canvasRaycaster.Raycast(pData, canvasHits);
                        for (int i = 0; i < canvasHits.Count; ++i)
                        {
                            Button button = canvasHits[i].gameObject.GetComponent<Button>();
                            if (button != null)
                            {
                                hitPos = uiRayCastOrigin + uiRayCastDirection * canvasHits[i].distance;
                                hitNormal = canvasHits[i].gameObject.transform.forward;
                                hitButton = button;
                                return true;
                            }
                        }

                        // No buttons, but hit canvas object
                        hitPos = hitInfo.point;
                        hitNormal = hitInfo.normal;
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckPlacement()
        {
            if ((rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor
                    || rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.FloorLike
                    || rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Platform
                    || rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Ceiling) && AppState.Instance.place)
            {
                bool flag = false;
                int pos = 0;
                //Debug.Log(rayCastResult.IntersectPoint.x);
                //Debug.Log(rayCastResult.IntersectPoint.z);
                for (int i = 0; i < locationCountFloor; i++)
                {

                    if ((Mathf.Abs(rayCastResult.IntersectPoint.x - resultsTopologyFloor[i].position.x) > 0.1f))
                    {
                        //Debug.Log(i);
                        continue;
                    }
                    if ((Mathf.Abs(rayCastResult.IntersectPoint.z - resultsTopologyFloor[i].position.z) > 0.1f))
                    {
                        //Debug.Log("2 " + i);
                        continue;
                    }
                    if ((Mathf.Abs(rayCastResult.IntersectPoint.y - resultsTopologyFloor[i].position.y) > halfHeight))
                    {
                        //Debug.Log("2 " + i);
                        continue;
                    }
                    Debug.Log(i);
                    flag = true;
                    pos = i;
                    break;
                }
                if (flag)
                {
                    Vector3 position = resultsTopologyFloor[pos].position;
                    Quaternion quat = Quaternion.LookRotation(Vector3.forward, resultsTopologyFloor[pos].normal);
                    //position.TransformPoint(Vector3.up * 0.2f, quat, Vector3.one);
                    cubePosition.transform.SetPositionAndRotation(position, Quaternion.identity);
                    cube.SetActive(true);
                }
            }
            else if ((rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallExternal
                        || rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallLike) && AppState.Instance.place)
            {
                bool flag = false;
                int pos = 0;
                //Debug.Log(rayCastResult.IntersectPoint.x);
                //Debug.Log(rayCastResult.IntersectPoint.z);
                for (int i = 0; i < locationCountWall; i++)
                {

                    if ((Mathf.Abs(rayCastResult.IntersectPoint.x - resultsTopologyWall[i].position.x) > halfWidth))
                    {
                        //Debug.Log(i);
                        continue;
                    }
                    if ((Mathf.Abs(rayCastResult.IntersectPoint.z - resultsTopologyWall[i].position.z) > halfHeight))
                    {
                        //Debug.Log("2 " + i);
                        continue;
                    }
                    if ((Mathf.Abs(rayCastResult.IntersectPoint.y - resultsTopologyWall[i].position.y) > halfHeight))
                    {
                        //Debug.Log("2 " + i);
                        continue;
                    }
                    Debug.Log(i);
                    flag = true;
                    pos = i;
                    break;
                }
                if (flag)
                {
                    Vector3 position = resultsTopologyWall[pos].position;
                    Quaternion quat = Quaternion.LookRotation(Vector3.up, resultsTopologyWall[pos].normal);
                    position.TransformPoint(Vector3.up * 0.2f, quat, Vector3.one);
                    cubePosition.transform.SetPositionAndRotation(position, quat);
                    cube.SetActive(true);
                }
            }
            else
            {
                cube.SetActive(false);
            }
        }

        public float refreshRate = 0.5f;
        private float nextRefresh = 0.0f;

        private void TestPlacment()
        {
            //if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor) return;

            if(Time.time < nextRefresh)
            {
                return;
            }
            nextRefresh = Time.time + refreshRate;

            IntPtr test = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(testRes);

            testRes.normal = rayCastResult.IntersectNormal;
            testRes.position = rayCastResult.IntersectPoint;
            testRes.length = 0.2f;
            testRes.width = 0.2f;

            //Vector3 a, b, c, d;
            //a = testRes.position;
            Quaternion quat = Quaternion.LookRotation(CameraCache.Main.transform.forward, testRes.normal);
            quat = Quaternion.LookRotation(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)), testRes.normal);
            //if (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Floor)
            {
                Debug.Log(Vector3.Scale(CameraCache.Main.transform.forward, (Vector3.one - testRes.normal)));
                //return;
            }

            Vector3 topLeft = testRes.position + quat * new Vector3(-0.2f, 0f, -0.2f);
            Vector3 topRight = testRes.position + quat * new Vector3(-0.2f, 0f, 0.2f);
            Vector3 bottomLeft = testRes.position + quat * new Vector3(0.2f, 0f, -0.2f);
            Vector3 bottomRight = testRes.position + quat * new Vector3(0.2f, 0f, 0.2f);

            Debug.Log("ray:"+testRes.position + "\nnorm" + testRes.normal + "\nTL:" + topLeft + "\nTR:" + topRight + "\nBR:" + bottomRight + "\nBL:" + bottomLeft);

            bool b = SpatialUnderstandingDllTopology.QueryTopology_IsValidRect(topLeft, topRight, bottomLeft, bottomRight, requestType, (int)rayCastResult.SurfaceType, test);

            //bool b = SpatialUnderstandingDllTopology.QueryTopology_IsValidFloorRect(topLeft, topRight, bottomLeft, bottomRight, requestType, test);
            if (b)
            {
                CursorText.text = "ok";
                cubePosition.transform.SetPositionAndRotation(testRes.position, quat);
                cube.SetActive(true);
            }
            else
            {
                CursorText.text = "no";
                cube.SetActive(false);
            }
        }

        protected override void LateUpdate()
        {
            // Base
            base.LateUpdate();

            // Basic checks
            if ((SpatialUnderstanding.Instance == null) ||
               ((SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Scanning) &&
                (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Finishing) &&
                (SpatialUnderstanding.Instance.ScanState != SpatialUnderstanding.ScanStates.Done)))
            {
                CursorText.gameObject.SetActive(false);
                return;
            }
            if (!SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                return;
            }

            // Report the results
            if ((rayCastResult != null) &&
                (rayCastResult.SurfaceType != SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Invalid))
            {
                CursorText.gameObject.SetActive(true);
                //CursorText.text = rayCastResult.SurfaceType.ToString();

                TestPlacment();

                //CheckPlacement();

                //if((rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.Platform) && AppState.Instance.place)
                //{
                //    bool flag = false;
                //    int pos = 0;
                //    Debug.Log(rayCastResult.IntersectPoint.x);
                //    Debug.Log(rayCastResult.IntersectPoint.z);
                //    for (int i = 0; i < locationCountFloor; i++)
                //    {

                //        if ((Mathf.Abs(rayCastResult.IntersectPoint.x - resultsTopologyFloor[i].position.x) > 0.1f))
                //        {
                //            //Debug.Log(i);
                //            continue;
                //        }
                //        if ((Mathf.Abs(rayCastResult.IntersectPoint.z - resultsTopologyFloor[i].position.z) > 0.1f))
                //        {
                //            //Debug.Log("2 " + i);
                //            continue;
                //        }
                //        Debug.Log(i);
                //        flag = true;
                //        pos = i;
                //        break;
                //    }
                //    if(flag)
                //    {
                //        cube.transform.SetPositionAndRotation(resultsTopologyFloor[pos].position, cube.transform.rotation);
                //        cube.SetActive(true);
                //    }
                //}
                //else if ((rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallExternal
                //            || rayCastResult.SurfaceType == SpatialUnderstandingDll.Imports.RaycastResult.SurfaceTypes.WallLike) && AppState.Instance.place)
                //{
                //    bool flag = false;
                //    int pos = 0;
                //    Debug.Log(rayCastResult.IntersectPoint.x);
                //    Debug.Log(rayCastResult.IntersectPoint.z);
                //    for (int i = 0; i < locationCountWall; i++)
                //    {

                //        if ((Mathf.Abs(rayCastResult.IntersectPoint.x - resultsTopologyWall[i].position.x) > halfWidth))
                //        {
                //            //Debug.Log(i);
                //            continue;
                //        }
                //        if ((Mathf.Abs(rayCastResult.IntersectPoint.z - resultsTopologyWall[i].position.z) > halfHeight))
                //        {
                //            //Debug.Log("2 " + i);
                //            continue;
                //        }
                //        if ((Mathf.Abs(rayCastResult.IntersectPoint.y - resultsTopologyWall[i].position.y) > halfHeight))
                //        {
                //            //Debug.Log("2 " + i);
                //            continue;
                //        }
                //        Debug.Log(i);
                //        flag = true;
                //        pos = i;
                //        break;
                //    }
                //    if (flag)
                //    {
                //        Vector3 position = resultsTopologyWall[pos].position;
                //        Quaternion quat = Quaternion.LookRotation(Vector3.up, resultsTopologyWall[pos].normal);
                //        position.TransformPoint(Vector3.up * 0.2f, quat, Vector3.one);
                //        cube.transform.SetPositionAndRotation(position, quat);
                //        cube.SetActive(true);
                //    }
                //}
                //else
                //{
                //    cube.SetActive(false);
                //}

                CursorText.transform.rotation = Quaternion.LookRotation(CameraCache.Main.transform.forward, Vector3.up);
                CursorText.transform.position = transform.position + CameraCache.Main.transform.right * 0.05f;
            }
            else
            {
                CursorText.gameObject.SetActive(false);
            }

            // If we're looking at the UI, fade the text
            Vector3 hitPos, hitNormal;
            Button hitButton;
            float textAlpha = RayCastUI(out hitPos, out hitNormal, out hitButton) ? 0.15f : 1.0f;
            CursorText.color = new Color(1.0f, 1.0f, 1.0f, textAlpha);
        }
    }
}
