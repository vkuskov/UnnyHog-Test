using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour {

    [SerializeField]
    private AvatarController _avatar;

    [SerializeField]
    private Slider _bar;

    void Update () {
        var camera = Camera.main;
        transform.rotation = Quaternion.LookRotation((transform.position - camera.transform.position).normalized);
        _bar.value = _avatar.RelativeHealth;
    }
}
