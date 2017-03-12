using System;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(AvatarController))]
class PlayerController : MonoBehaviour {
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const float DEADZONE = 0.25f;
    private const float MAX_RAYCAST_DISTANCE = 100.0f;
    private const float MIN_MAGNITUDE = 1.0f;
    private const int LAYER_MASK = 1 << 9;

    private AvatarController _avatarController;

    [SerializeField]
    private Transform _cameraTarget;

    public void Awake() {
        _avatarController = GetComponent<AvatarController>();
    }

    public void Update() {
        if (!_avatarController.IsDead()) {
            updateMotion();
            updateFacing();
            updateFire();
        }
    }

    public void Start() {
        var allCameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var it in allCameras) {
            it.CameraTransposerTarget = transform;
            it.CameraComposerTarget = _cameraTarget;
        }
    }

    private void updateFire() {
        if (Input.GetMouseButtonDown(0)) {
            _avatarController.Fire();
            NetworkManager.Instance.ShootAnim();
        }
    }

    private void updateFacing() {
        var mouse = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,MAX_RAYCAST_DISTANCE, LAYER_MASK)) {
            var point = hit.point;
            var ourPosition = new Vector2(transform.position.x, transform.position.z);
            var targetPosition = new Vector2(point.x, point.z);
            var toTarget = targetPosition - ourPosition;
            if (toTarget.magnitude > MIN_MAGNITUDE) {
                _avatarController.FaceDirection(toTarget.normalized);
            }
        }
    }

    private void updateMotion() {
        var h = Input.GetAxisRaw(HORIZONTAL);
        var v = Input.GetAxisRaw(VERTICAL);
        var direction = new Vector2(h, v);
        if (direction.magnitude <= DEADZONE) {
            direction = Vector2.zero;
        } else {
            direction.Normalize();
        }
        _avatarController.Move(direction);
    }
}
