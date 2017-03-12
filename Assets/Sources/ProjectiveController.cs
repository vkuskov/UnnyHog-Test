using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectiveController : MonoBehaviour {
    private const float SPEED = 15.0f;
    private const float MAX_DISTANCE = 30.0f;
    private const int DAMAGE = 20;

    private float _distance;
    private bool _ours;
    private bool _impactHappened;

    private Rigidbody _rigidbody;

    public void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public static ProjectiveController CreateBullet(Vector3 position, Quaternion rotation, float traveledDistance, bool ours) {
        var resource = Resources.Load<ProjectiveController>("Fireball");
        var projective = GameObject.Instantiate<ProjectiveController>(resource);
        projective.transform.position = position;
        projective.transform.rotation = rotation;
        projective._distance = traveledDistance;
        projective._ours = ours;
        return projective;
    }

    public void Update () {
        var dt = Time.smoothDeltaTime;
        var distance = SPEED * dt;
        _rigidbody.MovePosition(_rigidbody.position + transform.forward * distance);
        _distance += distance;
        if (_distance >= MAX_DISTANCE) {
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if (!_impactHappened) {
            var avatar = collision.gameObject.GetComponent<AvatarController>();
            if (avatar != null) {
                avatar.Hit(DAMAGE);
            }
            if (collision.contacts.Length > 0) {
                Effect.PlayHitSound(collision.contacts[0].point);
                _impactHappened = true;
            }
            Destroy(gameObject);
        }
    }
}
