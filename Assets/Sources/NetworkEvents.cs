using System.IO;
using UnityEngine;

public enum EventType : byte {
    Spawn = 0,
    Sync,
    Shoot,
    ShootAnim,
    Death,    
}

public interface INetworkData {
    void Write(BinaryWriter writer);
    void Read(BinaryReader reader);
}

public struct FullState : INetworkData {
    public Vector3 position;
    public Quaternion rotation;
    public float speed;
    public int health;

    public void Write(BinaryWriter writer) {
        writer.Write(position);
        writer.Write(rotation);
        writer.Write(speed);
        writer.Write(health);
    }

    public void Read(BinaryReader reader) {
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
        speed = reader.ReadSingle();
        health = reader.ReadInt32();
    }
}

public struct ShootEvent : INetworkData {
    public Vector3 position;
    public Quaternion rotation;
    public float traveledDistance;

    public void Write(BinaryWriter writer) {
        writer.Write(position);
        writer.Write(rotation);
        writer.Write(traveledDistance);
    }

    public void Read(BinaryReader reader) {
        position = reader.ReadVector3();
        rotation = reader.ReadQuaternion();
        traveledDistance = reader.ReadSingle();
    }
}
