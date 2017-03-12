using System;
using System.Collections.Generic;
using UnityEngine;

public class Server : NetworkBase {
    private const int MAX_CONNECTIONS = 2;

    private List<int> _allConnections = new List<int>();

    public void Init(int port) {
        init(port, MAX_CONNECTIONS);
    }

    protected override void onConnected(int hostId, int connectionId) {
        if (!_allConnections.Contains(connectionId)) {
            _allConnections.Add(connectionId);
        }
    }

    protected override void onDeath(int connectionId) {
        foreach (var it in _allConnections) {
            sendDeathEvent(it);
        }
    }

    protected override void onDisconnected(int hostId, int connectionId) {
        _allConnections.Remove(connectionId);
    }

    protected override void onShoot(int connectionId, ShootEvent data) {
        foreach (var it in _allConnections) {
            if (it != connectionId) {
                sendShootEvent(it, data);
            }
        }
    }

    protected override void onSpawn(int connectionId, FullState data) {
        foreach (var it in _allConnections) {
            if (it != connectionId) {
                sendSpawnEvent(it, data);
            }
        }
    }

    protected override void onSync(int connectionId, FullState data) {
        foreach (var it in _allConnections) {
            if (it != connectionId) {
                sendSyncEvent(it, data);
            }
        }
    }

    protected override void onShootAnim(int connectionId) {
        foreach (var it in _allConnections) {
            if (it != connectionId) {
                sendShootAnimEvent(it);
            }
        }
    }

    protected override bool canSendData() {
        return true;
    }
}

