using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Data;

public static class TilemapShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 36)]
    public struct InstanceData(Vector3 positionLast, Vector3 positionCurrent, int index, int frameCount, int frameTime) {
        [FieldOffset(0)] public Vector3 PositionLast = positionLast;
        [FieldOffset(12)] public Vector3 PositionCurrent = positionCurrent;
        [FieldOffset(24)] public int Index = index;
        [FieldOffset(28)] public int FrameCount = frameCount;
        [FieldOffset(32)] public int FrameTime = frameTime;
    }
}