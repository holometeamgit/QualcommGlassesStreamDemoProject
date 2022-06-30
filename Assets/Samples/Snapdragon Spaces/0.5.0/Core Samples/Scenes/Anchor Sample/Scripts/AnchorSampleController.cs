/******************************************************************************
 * File: AnchorSampleController.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class AnchorSampleController : SampleController
    {
        public GameObject GizmoTransparent;
        public GameObject GizmoTrackedAnchor;
        public GameObject GizmoUntrackedAnchor;
        public GameObject GizmoTrackedSession;
        public GameObject CreateButtonUI;
        
        public InputActionReference TriggerAction;

        private float _placementDistance = 1f;
        private bool _placementMode = true;
        private Transform _cameraTransform;
        private ARAnchorManager _arAnchorManager;
        private GameObject _indicatorGizmo;
        private List<GameObject> _gizmos = new List<GameObject>();

        public override void Start() {
            base.Start();
            _cameraTransform = Camera.main.transform;
            _indicatorGizmo = Instantiate(GizmoTransparent, transform.position, Quaternion.identity);

            TriggerAction.action.performed += OnTriggerAction;

            FindObjectOfType<ARAnchorManager>().anchorsChanged += OnAnchorsChanged;
        }

        public override void OnEnable() {
            base.OnEnable();
            SwitchInputAction.action.performed += UpdateCreateButtonUI;
        }

        public override void OnDisable() {
            base.OnDisable();
            SwitchInputAction.action.performed -= UpdateCreateButtonUI;
        }

        private void OnTriggerAction(InputAction.CallbackContext context) {
            if (_placementMode) {
                InstantiateGizmos();
                _indicatorGizmo.SetActive(false);
            } else {
                _indicatorGizmo.SetActive(true);
            }

            _placementMode = !_placementMode;
        }

        private void OnAnchorsChanged(ARAnchorsChangedEventArgs args) {
            foreach (var anchor in args.updated) {
                Destroy(anchor.transform.GetChild(0).gameObject);
                var newGizmo = Instantiate(anchor.trackingState == TrackingState.None ? GizmoUntrackedAnchor : GizmoTrackedAnchor);
                newGizmo.transform.SetParent(anchor.transform, false);
            }
        }

        public override void Update() {
            base.Update();
            if (_placementMode) {   
                _indicatorGizmo.transform.position = _cameraTransform.position + _cameraTransform.forward * _placementDistance;
            }
        }
        
        public void InstantiateGizmos() {
            if (_placementMode) {
                var sessionGizmo =
                    Instantiate(GizmoTrackedSession, _indicatorGizmo.transform.position, Quaternion.identity);
                _gizmos.Add(sessionGizmo);

                var anchorGizmo = Instantiate(new GameObject(), _indicatorGizmo.transform.position, Quaternion.identity);
                ARAnchor anchor = anchorGizmo.AddComponent<ARAnchor>();
                anchor.destroyOnRemoval = true;
                Instantiate(GizmoUntrackedAnchor).transform.SetParent(anchorGizmo.transform, false);
                _gizmos.Add(anchorGizmo);
            }
        }

        public void DestroyGizmos() {
            StartCoroutine(DestroyGizmosCoroutine());
        }

        private IEnumerator DestroyGizmosCoroutine() {
            yield return new WaitForEndOfFrame();
            foreach (var gizmo in _gizmos.ToList()) {
                Destroy(gizmo);
            }
            _gizmos.Clear();
        }

        private void UpdateCreateButtonUI(InputAction.CallbackContext ctx) {
            _placementMode = true;
            CreateButtonUI.SetActive(GazePointer.activeSelf);
        }
    }
}
