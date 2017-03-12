using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : NetworkBase {
    private const string CHARACTER_RESOURCE = "SlaveCharacter";

    private const int MIN_CLIENT_PORT = 5555;
    private const int MAX_CLIENT_PORT = 9999;

    private int _connectionId;
    private INetworkEventListener _listener;
    private bool _isConnected;

    private Dictionary<int, AvatarController> _connectedAvatars = new Dictionary<int, AvatarController>();

    public Client(INetworkEventListener listener) {
        _listener = listener;
    }

    public void Init(string host, int serverPort) {
        init(MIN_CLIENT_PORT, MAX_CLIENT_PORT, 1);
        byte error;
        _connectionId = NetworkTransport.Connect(_hostId, host, serverPort, 0, out error);
        var netError = (NetworkError)error;
        if (netError != NetworkError.Ok) {
            Debug.LogErrorFormat("Connect to {0}: {1}", host, error);
        }
    }

    public void Disconnect() {
        byte error;
        NetworkTransport.Disconnect(_hostId, _connectionId, out error);
        var netError = (NetworkError)error;
        if (netError != NetworkError.Ok) {
            Debug.LogErrorFormat("Disconnect: {0}", netError);
        }
    }

    protected override void onConnected(int hostId, int connectionId) {
        if (_listener != null) {
            _listener.OnConnected();
        }
        _isConnected = true;
    }

    protected override void onDeath(int connectionId) {
        AvatarController avatar;
        if (_connectedAvatars.TryGetValue(connectionId, out avatar)) {
            GameObject.Destroy(avatar.gameObject);
        }
        _connectedAvatars.Remove(connectionId);
    }

    protected override void onDisconnected(int hostId, int connectionId) {
        if (_listener != null) {
            _listener.OnDisconnected();
        }
        _isConnected = false;
    }

    protected override void onShoot(int connectionId, ShootEvent data) {
        ProjectiveController.CreateBullet(data.position, data.rotation, data.traveledDistance, false);
    }

    protected override void onSpawn(int connectionId, FullState data) {
        if (_connectedAvatars.ContainsKey(connectionId)) {
            var avatar = _connectedAvatars[connectionId];
            GameObject.Destroy(avatar);
            _connectedAvatars.Remove(connectionId);
        }
        var resource = Resources.Load<AvatarController>(CHARACTER_RESOURCE);
        var character = GameObject.Instantiate<AvatarController>(resource);
        _connectedAvatars.Add(connectionId, character);
        character.Sync(data);
    }

    protected override void onSync(int connectionId, FullState data) {
        AvatarController avatar;
        if (_connectedAvatars.TryGetValue(connectionId, out avatar)) {
            avatar.Sync(data);
        } else {
            onSpawn(connectionId, data);
        }
    }

    protected override void onShootAnim(int connectionId) {
        AvatarController avatar;
        if (_connectedAvatars.TryGetValue(connectionId, out avatar)) {
            avatar.Fire();
        }
    }

    public void OnSpawn(FullState data) {
        sendSpawnEvent(_connectionId, data);
    }

    public void OnSync(FullState data) {
        sendSyncEvent(_connectionId, data);
    }

    public void OnShoot(ShootEvent data) {
        sendShootEvent(_connectionId, data);
    }

    public void OnDeath() {
        sendDeathEvent(_connectionId);
    }

    public void OnShootAnim() {
        sendShootAnimEvent(_connectionId);
    }

    protected override bool canSendData() {
        return _isConnected;
    }
}
