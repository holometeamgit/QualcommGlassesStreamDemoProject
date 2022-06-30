/******************************************************************************
 * File: PositionalTrackingSampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class PositionalTrackingSampleController : SampleController
    {
        /* Added in XYZ order */
        public Text[] PositionTexts;
        /* Added in XYZ order */
        public Text[] RotationTexts;
        public TrailRenderer HeadTrailRenderer;
        public GameObject Mirror;
        public GameObject MirroredPlayer;
        public GameObject MirroredPlayerHead;

        private Transform _mainCameraTransform;

        public override void Start() {
            base.Start();
            _mainCameraTransform = Camera.main.transform;
        }
        
        public override void Update() {
            base.Update();
            UpdatePositionAndRotationUI();
            UpdateMirroredPlayerPose();
        }

        private void UpdatePositionAndRotationUI() {
            /* Track head position and rotation in the UI */
            PositionTexts[0].text = _mainCameraTransform.position.x.ToString("#0.00");
            PositionTexts[1].text = _mainCameraTransform.position.y.ToString("#0.00");
            PositionTexts[2].text = _mainCameraTransform.position.z.ToString("#0.00");

            Vector3 rotation = _mainCameraTransform.eulerAngles;
            RotationTexts[0].text = rotation.x.ToString("#0.0");
            RotationTexts[1].text = rotation.y.ToString("#0.0");
            RotationTexts[2].text = rotation.z.ToString("#0.0");
        }

        public void OnHeadTrailToggled() {
            HeadTrailRenderer.emitting = !HeadTrailRenderer.emitting;
        }

        private void UpdateMirroredPlayerPose() {
            /* Maths for reflection across a line can be found here: https://en.wikipedia.org/wiki/Reflection_(mathematics) */
            /* World space normal from the mirror plane. */
            var normal = Mirror.transform.forward;
            /* Position to be reflected in a hyperplane through the origin. Therefore offset, the player position by the mirror position. */
            var adjustedPosition = _mainCameraTransform.position - Mirror.transform.position;
            var reflectedPosition = adjustedPosition - 2  * Vector3.Dot(adjustedPosition, normal) / Vector3.Dot(normal, normal) * normal;
            /* Offset the origin of the reflection again by the mirror position. */
            MirroredPlayer.transform.position = Mirror.transform.position + reflectedPosition;

            var reflectedForward = Vector3.Reflect(_mainCameraTransform.transform.rotation * Vector3.forward, normal);
            var reflectedUp = Vector3.Reflect(_mainCameraTransform.transform.rotation * Vector3.up, normal);
            MirroredPlayerHead.transform.rotation = Quaternion.LookRotation(reflectedForward, reflectedUp);
        }
    }
}
