using UnityEngine;
using System.IO;

public static class Extensions {
    public static void Write(this BinaryWriter writer, Vector3 data) {
        writer.Write(data.x);
        writer.Write(data.y);
        writer.Write(data.z);
    }

    public static void Write(this BinaryWriter writer, Quaternion data) {
        writer.Write(data.x);
        writer.Write(data.y);
        writer.Write(data.z);
        writer.Write(data.w);
    }

    public static Vector3 ReadVector3(this BinaryReader reader) {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        return new Vector3(x, y, z);
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader) {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();
        var z = reader.ReadSingle();
        var w = reader.ReadSingle();
        return new Quaternion(x, y, z, w);
    }
}
