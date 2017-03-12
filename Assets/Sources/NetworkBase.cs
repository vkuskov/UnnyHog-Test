using System;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public abstract class NetworkBase {
    private const int BUFFER_SIZE = 256;
    protected int _hostId;
    protected int _port;
    protected int _eventChannelId;
    protected int _deltaChannelId;
    private byte[] _buffer = new byte[BUFFER_SIZE];

    public void Handle() {
        NetworkEventType recNetworkEvent = NetworkEventType.Nothing;
        do {
            int recConnectionId;
            int recChannelId;
            int dataSize;
            byte error;
            recNetworkEvent = NetworkTransport.ReceiveFromHost(_hostId, out recConnectionId, out recChannelId, _buffer, BUFFER_SIZE, out dataSize, out error);
            var netError = (NetworkError)error;
            if (netError != NetworkError.Ok) {
                Debug.LogErrorFormat("Receive: {0}", netError);
                return;
            }
            switch (recNetworkEvent) {
                case NetworkEventType.ConnectEvent:
                    onConnected(_hostId, recConnectionId);
                    break;
                case NetworkEventType.DisconnectEvent:
                    onDisconnected(_hostId, recConnectionId);
                    break;
                case NetworkEventType.DataEvent:
                    handleData(_hostId, recConnectionId, _buffer, dataSize);
                    break;
            }
        } while (recNetworkEvent != NetworkEventType.Nothing);
        
    }

    private HostTopology getTopology(int maxConnections) {
        var config = new ConnectionConfig();
        _eventChannelId = config.AddChannel(QosType.Reliable);
        _deltaChannelId = config.AddChannel(QosType.Unreliable);
        return new HostTopology(config, maxConnections);
    }

    protected void init(int port, int maxConnections) {
        var topology = getTopology(maxConnections);
        _hostId = NetworkTransport.AddHost(topology, port);
    }

    protected void init(int minPort, int maxPort, int maxConnections) {
        var topology = getTopology(maxConnections);
        var hostId = -1;
        while (hostId < 0) {
            var port = UnityEngine.Random.Range(minPort, maxPort);
            hostId = NetworkTransport.AddHost(topology, port);
        }
        _hostId = hostId;
    }

    private void handleData(int hostId, int connectionId, byte[] data, int dataSize) {
        using (var stream = new MemoryStream(data, 0, dataSize)) {
            using (var reader = new BinaryReader(stream)) {
                var eventType = (EventType)reader.ReadByte();
                switch (eventType) {
                    case EventType.Death:
                        onDeath(connectionId);
                        break;
                    case EventType.ShootAnim:
                        onShootAnim(connectionId);
                        break;
                    case EventType.Shoot:
                        var shootEvent = new ShootEvent();
                        shootEvent.Read(reader);
                        onShoot(connectionId, shootEvent);
                        break;
                    case EventType.Spawn:
                        var spawnData = new FullState();
                        spawnData.Read(reader);
                        onSpawn(connectionId, spawnData);
                        break;
                    case EventType.Sync:
                        var syncData = new FullState();
                        syncData.Read(reader);
                        onSync(connectionId, syncData);
                        break;
                }
            }
        }
    }

    protected void sendEmptyEvent(int connectionId, int channelId, EventType type) {
        using (var stream = new MemoryStream()) {
            using (var writer = new BinaryWriter(stream)) {
                writer.Write((byte)type);
                var buffer = stream.ToArray();
                sendData(connectionId, channelId, buffer, buffer.Length);
            }
        }
    }

    private void sendData(int connectionId, int channelId, byte[] buffer, int size) {
        if (canSendData()) {
            byte error;
            NetworkTransport.Send(_hostId, connectionId, channelId, buffer, size, out error);
            var netError = (NetworkError)error;
            if (netError != NetworkError.Ok) {
                Debug.LogErrorFormat("Send: {0} Connection: {1} Channel: {2}", netError, connectionId, channelId);
            }
        }
    }

    protected void sendSpawnEvent(int connectionId, FullState data) {
        sendData(connectionId, _eventChannelId, EventType.Spawn, data); 
    }

    protected void sendSyncEvent(int connectionId, FullState data) {
        sendData(connectionId, _deltaChannelId, EventType.Sync, data);
    }

    protected void sendShootEvent(int connectionId, ShootEvent data) {
        sendData(connectionId, _eventChannelId, EventType.Shoot, data);
    }

    protected void sendDeathEvent(int connectionId) {
        sendEmptyEvent(connectionId, _eventChannelId, EventType.Death);
    }

    protected void sendShootAnimEvent(int connectionId) {
        sendEmptyEvent(connectionId, _deltaChannelId, EventType.ShootAnim);
    }

    private void sendData(int connectionId, int channelId, EventType type, INetworkData data) {
        using (var stream = new MemoryStream()) {
            using (var writer = new BinaryWriter(stream)) {
                writer.Write((byte)type);
                data.Write(writer);
                var buffer = stream.ToArray();
                sendData(connectionId, channelId, buffer, buffer.Length);
            }
        }
    }

    protected abstract void onConnected(int hostId, int connectionId);

    protected abstract void onDisconnected(int hostId, int connectionId);

    protected abstract void onDeath(int connectionId);

    protected abstract void onShootAnim(int connectionId);

    protected abstract void onShoot(int connectionId, ShootEvent data);

    protected abstract void onSpawn(int connectionId, FullState data);

    protected abstract void onSync(int connectionId, FullState data);

    protected abstract bool canSendData();
}

