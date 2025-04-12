using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Data;

public static class TilemapShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    public struct InstanceData(Vector3 position, int index, int frameCount, int frameTime) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public int Index = index;
        [FieldOffset(16)] public int FrameCount = frameCount;
        [FieldOffset(20)] public int FrameTime = frameTime;
    }
}