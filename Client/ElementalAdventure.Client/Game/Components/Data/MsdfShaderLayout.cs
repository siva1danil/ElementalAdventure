using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Components.Data;

public class MsdfShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 28)]
    public struct InstanceData(Vector3 position, Vector2 scale, Vector4 uv) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public Vector2 Scale = scale;
        [FieldOffset(20)] public Vector4 UV = uv;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct UniformData(Matrix4 projection) {
        [FieldOffset(0)] public Matrix4 Projection = projection;
    }
}