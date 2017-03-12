using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AvatarController : MonoBehaviour {
    private const string SPEED_PARAM = "Speed";
    private const string DEATH_TRIGGER = "Death";
    public const string FIRE_TRIGGER = "Fire";
    private const string HIT_TRIGGER = "Hit";
    private const float ACCELERATION = 5.0f;
    private const float DEACCELERATION = 10.0f;
    private const float MAX_SPEED = 7.5f;
    private const int MAX_HEALTH = 100;

    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private AudioSource _castSound;
    [SerializeField]
    private bool _real;

    private CharacterController _characterController;

    private float _currentSpeed;
    private float _desiredSpeed;
    private Quaternion _desiredRotation;
    private Quaternion _currentRotation;
    private int _health;

    private Vector2 _moveDirection;

    public float RelativeHealth {
        get {
            return (float)_health / MAX_HEALTH;
        }
    }

    public bool IsDead() {
        return _health == 0;
    }

    public void Awake() {
        _characterController = GetComponent<CharacterController>();
        _currentRotation = transform.rotation;
    }

    public void Start() {
        _health = MAX_HEALTH;
    }

    public void FaceDirection(Vector2 direction) {
        _desiredRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
    }

    public void Fire() {
        _animator.SetBool(FIRE_TRIGGER, true);
        _castSound.Play();
    }

    private void death() {
        _animator.SetTrigger(DEATH_TRIGGER);
    }

    public void Hit(int damage) {
        _animator.SetTrigger(HIT_TRIGGER);
        _health -= damage;
        _health = Mathf.Clamp(_health, 0, MAX_HEALTH);
        if (_health == 0) {
            death();
        }
    }

    public void Bury() {
        if (_real) {
            NetworkManager.Instance.Death();
            Destroy(gameObject);
        }
    }

    public void Move(Vector2 direction) {
        _desiredSpeed = direction.magnitude * MAX_SPEED;
        _moveDirection = direction;
    }

    public void Sync(FullState state) {
        _currentSpeed = state.speed;
        _currentRotation = state.rotation;
        transform.position = state.position;
        transform.rotation = state.rotation;
        _health = state.health;
    }

    public FullState GetState() {
        return new FullState() {
            position = transform.position,
            rotation = transform.rotation,
            speed = _currentSpeed,
            health = _health
        };
    }

    void Update () {
        if (_real) {
            var dt = Time.smoothDeltaTime;
            if (_currentSpeed < _desiredSpeed) {
                _currentSpeed = calculateSpeed(_currentSpeed, _desiredSpeed, ACCELERATION, dt);
            } else {
                _currentSpeed = calculateSpeed(_currentSpeed, _desiredSpeed, DEACCELERATION, dt);
            }
            _currentRotation = Quaternion.Slerp(_currentRotation, _desiredRotation, 0.5f);
            _characterController.Move(new Vector3(_moveDirection.x, 0, _moveDirection.y) * _currentSpeed * dt);
            transform.rotation = _currentRotation;
        }
        _animator.SetFloat(SPEED_PARAM, _currentSpeed / MAX_SPEED);
        
    }

    private float calculateSpeed(float current, float desired, float acceleration, float dt) {
        return Mathf.MoveTowards(current, desired, acceleration * dt);
    }
}
