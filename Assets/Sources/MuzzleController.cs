using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleController : MonoBehaviour {

    [SerializeField]
    private Transform _muzzle;

    public void DoFire() {
        var projective = ProjectiveController.CreateBullet(_muzzle.position, _muzzle.rotation, 0, true);
        NetworkManager.Instance.Shoot(new ShootEvent() {
            position = projective.transform.position,
            rotation = projective.transform.rotation,
            traveledDistance = 0
        });
    }
}
