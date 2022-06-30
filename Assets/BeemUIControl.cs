using UnityEngine;

public class BeemUIControl : MonoBehaviour {
    [SerializeField]
    private GameObject hologramObject;

    private bool moveLeft;
    private bool moveRight;
    private bool moveUp;
    private bool moveDown;
    private bool scaleUp;
    private bool scaleDown;
    private bool moveTowards;
    private bool moveAway;

    private int moveSpeed = 2;

    void Start() {

        if (hologramObject == null) {
            hologramObject = GameObject.Find("HoloMeParent");
        }
    }

    private void Update() {
        if (moveUp) {
            MoveUp();
        }

        if (moveDown) {
            MoveDown();
        }

        if (moveLeft) {
            MoveLeft();
        }

        if (moveRight) {
            MoveRight();
        }

        if (scaleUp) {
            ScaleUp();
        }

        if (scaleDown) {
            ScaleDown();
        }

        if (moveTowards) {
            MoveTowards();
        }

        if (moveAway) {
            MoveAway();
        }
    }

    public void SetUpFlag(bool move) => moveUp = move;
    public void SetDownFlag(bool move) => moveDown = move;
    public void SetLeftFlag(bool move) => moveLeft = move;
    public void SetRightFlag(bool move) => moveRight = move;
    public void SetScaleUpFlag(bool move) => scaleUp = move;
    public void SetScaleDownFlag(bool move) => scaleDown = move;
    public void SetMoveAwayFlag(bool move) => moveAway = move;
    public void SetMoveTowardsFlag(bool move) => moveTowards = move;

    private void ScaleUp() {
        hologramObject.transform.localScale = hologramObject.transform.localScale + new Vector3(0.25f, 0.25f, 0.25f) * Time.deltaTime;
    }
    private void ScaleDown() {
        hologramObject.transform.localScale = hologramObject.transform.localScale + new Vector3(-0.25f, -0.25f, -0.25f) * Time.deltaTime;
    }

    private void MoveTowards() {
        hologramObject.transform.LookAt(Camera.main.transform);
        hologramObject.transform.Translate(hologramObject.transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveAway() {
        hologramObject.transform.LookAt(Camera.main.transform);
        hologramObject.transform.Translate(-hologramObject.transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveDown() {
        hologramObject.transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveUp() {
        hologramObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveRight() {
        hologramObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);
    }

    private void MoveLeft() {
        hologramObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
    }
}
