/******************************************************************************
 * File: SampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR.ARFoundation;
using Vector3 = UnityEngine.Vector3;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
	public class SampleController : MonoBehaviour
	{
		public GameObject GazePointer;
		public GameObject DevicePointer;
		public InputActionReference SwitchInputAction;

		protected virtual bool ResetSessionOriginOnStart => true;

		private bool _isSessionOriginMoved = false;
		private Transform _camera;

		public virtual void Start() {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			_camera = Camera.main.transform;
		}

		public virtual void Update() {
			if (ResetSessionOriginOnStart && !_isSessionOriginMoved && _camera.position != Vector3.zero) {
				OffsetSessionOrigin();
				_isSessionOriginMoved = true;
			}
		}

		public void Quit() {
			Application.Quit();
		}

		protected void OffsetSessionOrigin() {
			ARSessionOrigin sessionOrigin = FindObjectOfType<ARSessionOrigin>();
			sessionOrigin.transform.Rotate(0.0f, -_camera.rotation.eulerAngles.y, 0.0f, Space.World);
			sessionOrigin.transform.position = -_camera.position;
		}

		public virtual void OnEnable() {
			SwitchInputAction.action.performed += OnSwitchInput;
		}

		public virtual void OnDisable() {
			SwitchInputAction.action.performed -= OnSwitchInput;
		}

		private void OnSwitchInput(InputAction.CallbackContext ctx) {
			if (ctx.interaction is TapInteraction) {
				GazePointer.SetActive(!GazePointer.activeSelf);
				DevicePointer.SetActive(!DevicePointer.activeSelf);
			}

			if (ctx.interaction is HoldInteraction) {
				Quit();
			}
		}
	}
}
