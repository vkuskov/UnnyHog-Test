using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarController))]
public class SyncPlayerState : MonoBehaviour {
    private const int SEND_FREQ = 20;

    private AvatarController _avatarController;

    private float _timeToNextSend = 0;

    void Awake() {
        _avatarController = GetComponent<AvatarController>();
    }
    
    void Update () {
        _timeToNextSend -= Time.smoothDeltaTime;
        if (_timeToNextSend <= 0) {
            NetworkManager.Instance.SyncPlayer(_avatarController.GetState());
            _timeToNextSend = 1.0f / SEND_FREQ;
        }        
    }
}
