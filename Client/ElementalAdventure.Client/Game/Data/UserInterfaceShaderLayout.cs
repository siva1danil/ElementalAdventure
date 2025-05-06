using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Data;

public static class UserInterfaceShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct InstanceData(Vector3 position, Vector2 scale, Vector3 color) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public Vector2 Scale = scale;
        [FieldOffset(20)] public Vector3 Color = color;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct UniformData(Matrix4 projection) {
        [FieldOffset(0)] public Matrix4 Projection = projection;
    }
}