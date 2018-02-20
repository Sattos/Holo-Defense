// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Examples.SpatialUnderstandingFeatureOverview
{
    public class BadUI : LineDrawer
    {
        // Consts
        public const float MenuWidth = 1.0f;
        public const float MenuHeight = 0.6f;
        public const float MenuMinDepth = 1.0f;


        // Config
        public Canvas ParentCanvas;
        public Button PrefabButton;
        public LayerMask UILayerMask;

        // Properties
        public bool HasPlacedMenu { get; private set; }
        public AnimatedBox MenuAnimatedBox { get; private set; }

        // Privates
        private DateTime timeLastQuery = DateTime.MinValue;
        private bool placedMenuNeedsBillboard = false;

        // Functions
        private void Start()
        {
            // Turn menu off until we're placed
            //ParentCanvas.gameObject.SetActive(false);
            //StartCoroutine(SetupMenu());
            // Events
            //SpatialUnderstanding.Instance.ScanStateChanged += OnScanStateChanged;
        }

        protected override void OnDestroy()
        {
            if (SpatialUnderstanding.Instance != null)
            {
                SpatialUnderstanding.Instance.ScanStateChanged -= OnScanStateChanged;
            }

            base.OnDestroy();
        }

        private void OnScanStateChanged()
        {
            // If we are leaving the None state, go ahead and register shapes now
            if ((SpatialUnderstanding.Instance.ScanState == SpatialUnderstanding.ScanStates.Done) &&
                SpatialUnderstanding.Instance.AllowSpatialUnderstanding)
            {
                // Make sure we've created our shapes
                ShapeDefinition.Instance.CreateShapes();

                // Make sure our solver is initialized
                LevelSolver.Instance.InitializeSolver();

                // Setup the menu
                StartCoroutine(SetupMenu());
            }
        }

        public void Setup()
        {
            StartCoroutine(SetupMenu());
        }

        private IEnumerator SetupMenu()
        {
            // Setup for queries
            SpatialUnderstandingDllTopology.TopologyResult[] resultsTopology = new SpatialUnderstandingDllTopology.TopologyResult[1];
            IntPtr resultsTopologyPtr = SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(resultsTopology);

            // Place on a wall (do it in a thread, as it can take a little while)
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition placeOnWallDef =
                SpatialUnderstandingDllObjectPlacement.ObjectPlacementDefinition.Create_OnWall(new Vector3(MenuWidth * 0.5f, MenuHeight * 0.5f, MenuMinDepth * 0.5f), 0.5f, 3.0f);
            SpatialUnderstandingDllObjectPlacement.ObjectPlacementResult placementResult = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResult();

            var thread =
#if UNITY_EDITOR || !UNITY_WSA
                new System.Threading.Thread
#else
                System.Threading.Tasks.Task.Run
#endif
            (() =>
            {
                if (SpatialUnderstandingDllObjectPlacement.Solver_PlaceObject(
                    "UIPlacement",
                    SpatialUnderstanding.Instance.UnderstandingDLL.PinObject(placeOnWallDef),
                    0,
                    IntPtr.Zero,
                    0,
                    IntPtr.Zero,
                    SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticObjectPlacementResultPtr()) == 0)
                {
                    placementResult = null;
                }
            });

#if UNITY_EDITOR || !UNITY_WSA
            thread.Start();
#endif

            while
                (
#if UNITY_EDITOR || !UNITY_WSA
                !thread.Join(TimeSpan.Zero)
#else
                !thread.IsCompleted
#endif
                )
            {
                yield return null;
            }
            if (placementResult != null)
            {
                Debug.Log("PlaceMenu - ObjectSolver-OnWall");
                Vector3 posOnWall = placementResult.Position - placementResult.Forward * MenuMinDepth * 0.5f;
                PlaceMenu(posOnWall, -placementResult.Forward);
                yield break;
            }

            // Wait a frame
            yield return null;

            Transform cameraTransform = CameraCache.Main.transform;
            // Fallback, place floor (add a facing, if so)
            int locationCount = SpatialUnderstandingDllTopology.QueryTopology_FindLargestPositionsOnFloor(
                resultsTopology.Length, resultsTopologyPtr);
            if (locationCount > 0)
            {
                Debug.Log("PlaceMenu - LargestPositionsOnFloor");
                SpatialUnderstandingDllTopology.TopologyResult menuLocation = resultsTopology[0];
                Vector3 menuPosition = menuLocation.position + Vector3.up * MenuHeight;
                Vector3 menuLookVector = cameraTransform.position - menuPosition;
                PlaceMenu(menuPosition, (new Vector3(menuLookVector.x, 0.0f, menuLookVector.z)).normalized, true);
                yield break;
            }

            // Final fallback just in front of the user
            SpatialUnderstandingDll.Imports.QueryPlayspaceAlignment(SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignmentPtr());
            SpatialUnderstandingDll.Imports.PlayspaceAlignment alignment = SpatialUnderstanding.Instance.UnderstandingDLL.GetStaticPlayspaceAlignment();
            Vector3 defaultPosition = cameraTransform.position + cameraTransform.forward * 2.0f;
            PlaceMenu(new Vector3(defaultPosition.x, Math.Max(defaultPosition.y, alignment.FloorYValue + 1.5f), defaultPosition.z), (new Vector3(cameraTransform.forward.x, 0.0f, cameraTransform.forward.z)).normalized, true);
            Debug.Log("PlaceMenu - InFrontOfUser");
        }

        private void PlaceMenu(Vector3 position, Vector3 normal, bool needsBillboarding = false)
        {
            // Offset in a bit
            position -= normal * 0.05f;
            Quaternion rotation = Quaternion.LookRotation(normal, Vector3.up);

            // Place it
            transform.position = position;
            transform.rotation = rotation;

            // Enable it
            ParentCanvas.gameObject.SetActive(true);

            // Create up a box
            MenuAnimatedBox = new AnimatedBox(0.0f, position, rotation, new Color(1.0f, 1.0f, 1.0f, 0.25f), new Vector3(MenuWidth * 0.5f, MenuHeight * 0.5f, 0.025f), LineDrawer.DefaultLineWidth * 0.5f);

            // Initial position
            transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
            transform.rotation = MenuAnimatedBox.Rotation * Quaternion.AngleAxis(360.0f * MenuAnimatedBox.AnimRotation.Evaluate(MenuAnimatedBox.Time), Vector3.up);

            // Billboarding (note that because of the transition animation we need to place this late)
            placedMenuNeedsBillboard = needsBillboarding;

            // And mark that we've done it
            HasPlacedMenu = true;
        }

        private void Update()
        {
            // Animated box
            if (MenuAnimatedBox != null)
            {
                // We're using the animated box for the animation only
                MenuAnimatedBox.Update(Time.deltaTime);

                // Billboarding
                if (MenuAnimatedBox.IsAnimationComplete &&
                    placedMenuNeedsBillboard)
                {
                    // Rotate to face the user
                    transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
                    Vector3 lookDirTarget = CameraCache.Main.transform.position - transform.position;
                    lookDirTarget = (new Vector3(lookDirTarget.x, 0.0f, lookDirTarget.z)).normalized;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-lookDirTarget), Time.deltaTime * 10.0f);
                }
                else
                {
                    // Keep the UI locked to the animated box
                    transform.position = MenuAnimatedBox.AnimPosition.Evaluate(MenuAnimatedBox.Time);
                    transform.rotation = MenuAnimatedBox.Rotation * Quaternion.AngleAxis(360.0f * MenuAnimatedBox.AnimRotation.Evaluate(MenuAnimatedBox.Time), Vector3.up);
                }
            }
        }
    }
}
