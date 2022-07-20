using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class BeemPlacementScript : MonoBehaviour {

    [SerializeField]
    private BeemScript beemScript;

    public GameObject DevicePointer;
    public GameObject PanelToToggle;

    public InputActionReference TriggerAction; //This is the select button
    public InputActionReference SwitchInputAction; //This is the menu button

    private float _placementDistance = 1f;
    private bool _placementMode = true;
    private Transform _cameraTransform;

    private GameObject _indicatorGizmo;
    private bool _isSessionOriginMoved = false;
    private Transform _camera;
    protected bool ResetSessionOriginOnStart => true;

    private Vector3 defaultPosition = new Vector3(0, -1.25f, 4);

    public void Start() {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _camera = Camera.main.transform;
        _cameraTransform = Camera.main.transform;

        //_indicatorGizmo = Instantiate(GizmoTransparent, transform.position, Quaternion.identity);

        TriggerAction.action.performed += OnTriggerAction;

        //FindObjectOfType<ARAnchorManager>().anchorsChanged += OnAnchorsChanged;
        DevicePointer.gameObject.SetActive(false);
        beemScript.ActivateHologram(defaultPosition);
    }

    public void ResetHologramPosition() {
        beemScript.MoveHologram(defaultPosition);
    }

    public void OnEnable() {
        SwitchInputAction.action.performed += UpdateCreateButtonUI;
    }

    public void OnDisable() {
        SwitchInputAction.action.performed -= UpdateCreateButtonUI;
    }

    private void OnTriggerAction(InputAction.CallbackContext context) {

        //beemScript.MoveHologram(_indicatorGizmo.transform.position);

        //if (_placementMode) {
        //    InstantiateGizmos();
        //    _indicatorGizmo.SetActive(false);
        //} else {
        //    _indicatorGizmo.SetActive(true);
        //}

        //_placementMode = !_placementMode;
    }

    public void Update() {

        if (ResetSessionOriginOnStart && !_isSessionOriginMoved && _camera.position != Vector3.zero) {
            OffsetSessionOrigin();
            _isSessionOriginMoved = true;
        }
    }

    protected void OffsetSessionOrigin() {
        ARSessionOrigin sessionOrigin = FindObjectOfType<ARSessionOrigin>();
        sessionOrigin.transform.Rotate(0.0f, -_camera.rotation.eulerAngles.y, 0.0f, Space.World);
        sessionOrigin.transform.position = -_camera.position;
    }

    //public void InstantiateGizmos() {


    //    if (_placementMode) {


    //        var sessionGizmo =
    //            Instantiate(GizmoTrackedSession, _indicatorGizmo.transform.position, Quaternion.identity);
    //        _gizmos.Add(sessionGizmo);

    //        var anchorGizmo = Instantiate(new GameObject(), _indicatorGizmo.transform.position, Quaternion.identity);
    //        ARAnchor anchor = anchorGizmo.AddComponent<ARAnchor>();
    //        anchor.destroyOnRemoval = true;
    //        Instantiate(GizmoUntrackedAnchor).transform.SetParent(anchorGizmo.transform, false);
    //        _gizmos.Add(anchorGizmo);
    //    }
    //}


    private void UpdateCreateButtonUI(InputAction.CallbackContext ctx) {
        _placementMode = true;
        PanelToToggle.SetActive(!PanelToToggle.activeInHierarchy);
        DevicePointer.SetActive(PanelToToggle.activeInHierarchy);
    }
}
