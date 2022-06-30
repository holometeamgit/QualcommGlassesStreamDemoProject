/******************************************************************************
 * File: EditorCameraController.cs
 * Copyright (c) 2021 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 * 
 * Confidential and Proprietary - Qualcomm Technologies, Inc.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class EditorCameraController : MonoBehaviour
    {
#if UNITY_EDITOR
        public GameObject GazePointer;
        public Transform DeviceController;
        public InputActionAsset InputActionControls;
        
        public float MoveSpeed = 2.0f;
        public float RotationSensitivity = 5.0f;
        
        private InputAction _cameraTranslationAction;
        private InputAction _cameraRotationAction;
        private InputAction _controllerMenuAction;

        private bool _isGazeControl = true;
        private Vector3 _moveDirection;
        private Vector2 _mouseDelta = Vector2.zero;

        private Camera _mainCamera;

        private void Start() {
            _mainCamera = Camera.main;

            /* Setup the input actions for translation and rotation */
            InputActionMap actionMap = InputActionControls.FindActionMap("EditorCamera");
            InputActionMap controllerMap = InputActionControls.FindActionMap("Pointer");

            _cameraTranslationAction = actionMap.FindAction("Translate");
            _cameraTranslationAction.performed += OnTranslate;
            _cameraTranslationAction.canceled += OnTranslate;
            
            _cameraRotationAction = actionMap.FindAction("Rotate");
            _cameraRotationAction.performed += OnRotate;
            _cameraRotationAction.canceled -= OnRotate;

            _controllerMenuAction = controllerMap.FindAction("Menu");
            _controllerMenuAction.performed += delegate{ SwitchInputModes(); };

            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Press Key " + Keyboard.current[Key.LeftShift].name + " to switch between gaze input and the simulated device controller.");
        }

        private void LateUpdate() {
            Vector3 moveDelta = (_mainCamera.transform.right * _moveDirection.x + _mainCamera.transform.forward * _moveDirection.z) * Time.deltaTime * MoveSpeed;
            _mainCamera.transform.Translate(moveDelta, Space.World);
            
            if (_isGazeControl) {
                UpdateMouseRotation();
            } else {
                UpdateDeviceControllerRotation();
            }
        }

        private void UpdateMouseRotation() {
            float pitch = _mouseDelta.y * RotationSensitivity * Time.fixedDeltaTime;
            float yaw = _mouseDelta.x * RotationSensitivity * Time.fixedDeltaTime;

            _mainCamera.transform.localRotation =
     Quaternion.AngleAxis(yaw, Vector3.up) * _mainCamera.transform.localRotation * Quaternion.AngleAxis(pitch, Vector3.left);

            _mouseDelta = Vector2.zero;
        }
        
        private void UpdateDeviceControllerRotation() {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition + Vector3.forward);

            DeviceController.LookAt(mousePosition);
        }
        
        private void OnTranslate(InputAction.CallbackContext context) {
            var value = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(value.x, 0.0f, value.y).normalized;
        }

        private void OnRotate(InputAction.CallbackContext context) {
            /* Only rotate if gaze control is active. */
            _mouseDelta = _isGazeControl ? _mouseDelta + context.ReadValue<Vector2>() : Vector2.zero;
        }
        
        private void SwitchInputModes() {
            _isGazeControl = !_isGazeControl;

            GazePointer?.SetActive(_isGazeControl);
            DeviceController?.gameObject.SetActive(!_isGazeControl);

            Cursor.lockState = _isGazeControl ? CursorLockMode.Locked : CursorLockMode.None;
        }
#endif
    }
}
