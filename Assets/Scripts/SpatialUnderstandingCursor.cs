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

        private GameObject currentRaycastTarget = null;
        public IRaycastFocusEvent currentRaycastFocus = null;
        public bool isPlacementBlocked;
        public ObjectPlacer.ObjectsToPlace towerType;

        public BlockingType blockingType;

        public void Click()
        {
            
            //if(currentRaycastFocus != null)
            //{
            //    Debug.Log(currentRaycastTarget.gameObject.name);
            //    if (ObjectPlacer.Instance.isPlacing)
            //    {
            //        switch(currentRaycastFocus.BlockingType)
            //        {
            //            case BlockingType.NotBlocking:
            //                ObjectPlacer.Instance.FinalizePlacement();
            //                break;
            //            case BlockingType.BlockingButton:
            //                //blockingType = currentRaycastFocus.BlockingType;
            //                //currentRaycastFocus.Click();
            //                break;
            //            case BlockingType.BlockingPlacement:

            //                //break;
            //            case BlockingType.OtherTower:
            //                blockingType = currentRaycastFocus.BlockingType;
            //                currentRaycastFocus.Click();
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        blockingType = currentRaycastFocus.BlockingType;
            //        currentRaycastFocus.Click();
            //    }
            //}
            //else
            //{
            //    if (ObjectPlacer.Instance.isPlacing)
            //    {
            //        ObjectPlacer.Instance.FinalizePlacement();
            //    }
            //}
        }

        public bool RaycastTargetChanged(GameObject target)
        {
            if (currentRaycastTarget == target)
            {
                return false;
            }

            if(currentRaycastFocus != null)
            {
                currentRaycastFocus.Deactivate();
            }

            currentRaycastTarget = target;

            if(currentRaycastTarget == null)
            {
                currentRaycastFocus = null;
                isPlacementBlocked = false;
                //towerType = ObjectPlacer.ObjectsToPlace.none;
                return false;
            }
            Debug.Log(currentRaycastTarget.gameObject.name);
            currentRaycastFocus = currentRaycastTarget.GetComponent<IRaycastFocusEvent>();

            if(currentRaycastFocus != null)
            {
                Debug.Log(currentRaycastTarget.gameObject.name);
                currentRaycastFocus.Activate();
                switch(currentRaycastFocus.BlockingType)
                {
                    case BlockingType.NotBlocking:
                        isPlacementBlocked = false;
                        break;
                    case BlockingType.BlockingButton:
                        isPlacementBlocked = false;
                        break;
                    case BlockingType.BlockingPlacement:
                        isPlacementBlocked = true;
                        break;
                    case BlockingType.OtherTower:
                        break;
                }
                //isPlacementBlocked = currentRaycastFocus.BlockPlacement;
                //towerType = currentRaycastFocus.BlockingType;
                return true;
            }
            //towerType = ObjectPlacer.ObjectsToPlace.none;
            isPlacementBlocked = false;
            return false;
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

        public float refreshRate = 0.5f;
        private float nextRefresh = 0.05f;

        private void TestPlacment()
        {
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

            Vector3 topLeft = testRes.position + quat * new Vector3(-0.2f, 0f, -0.2f);
            Vector3 topRight = testRes.position + quat * new Vector3(-0.2f, 0f, 0.2f);
            Vector3 bottomLeft = testRes.position + quat * new Vector3(0.2f, 0f, -0.2f);
            Vector3 bottomRight = testRes.position + quat * new Vector3(0.2f, 0f, 0.2f);

            Debug.Log("ray:"+testRes.position + "\nnorm" + testRes.normal + "\nTL:" + topLeft + "\nTR:" + topRight + "\nBR:" + bottomRight + "\nBL:" + bottomLeft);

            bool isValidRect = SpatialUnderstandingDllTopology.QueryTopology_IsValidRect(topLeft, topRight, bottomLeft, bottomRight, requestType, (int)rayCastResult.SurfaceType, test);

            if (isValidRect)
            {
                cubePosition.transform.SetPositionAndRotation(testRes.position, quat);
                cube.SetActive(true);
            }
            else
            {
                cube.SetActive(false);
            }
            CursorText.text = rayCastResult.SurfaceType.ToString();
        }

        void Update()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(
                    Camera.main.transform.position,
                    Camera.main.transform.forward,
                    out hitInfo,
                    20.0f,
                    UILayerMask))
            {
                // If the Raycast has succeeded and hit a hologram
                // hitInfo's point represents the position being gazed at
                // hitInfo's collider GameObject represents the hologram being gazed at
                Debug.Log(hitInfo.collider.gameObject.name);
                RaycastTargetChanged(hitInfo.collider.gameObject);
            }
            else
            {
                RaycastTargetChanged(null);
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

                //TestPlacment();
                if(ObjectPlacer.Instance.isPlacing)
                    ObjectPlacer.Instance.CheckPlacement(rayCastResult);

                CursorText.transform.rotation = Quaternion.LookRotation(CameraCache.Main.transform.forward, Vector3.up);
                CursorText.transform.position = transform.position + CameraCache.Main.transform.right * 0.05f;
            }
            else
            {
                CursorText.gameObject.SetActive(false);
            }

            // If we're looking at the UI, fade the text
            //Vector3 hitPos, hitNormal;
            //Button hitButton;
            //float textAlpha = RayCastUI(out hitPos, out hitNormal, out hitButton) ? 0.15f : 1.0f;
            //CursorText.color = new Color(1.0f, 1.0f, 1.0f, textAlpha);
            //RaycastHit hitInfo;
            //if (Physics.Raycast(
            //        Camera.main.transform.position,
            //        Camera.main.transform.forward,
            //        out hitInfo,
            //        20.0f,
            //        UILayerMask))
            //{
            //    // If the Raycast has succeeded and hit a hologram
            //    // hitInfo's point represents the position being gazed at
            //    // hitInfo's collider GameObject represents the hologram being gazed at

            //    RaycastTargetChanged(hitInfo.collider.gameObject);
            //}
            //else
            //{
            //    RaycastTargetChanged(null);
            //}
        }
    }
}
