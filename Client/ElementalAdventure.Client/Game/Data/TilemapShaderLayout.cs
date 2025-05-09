using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Data;

public static class TilemapShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 44)]
    public struct InstanceData(Vector3 positionLast, Vector3 positionCurrent, int frameIndex, Vector2i frameSize, int frameCount, int frameTime) {
        [FieldOffset(0)] public Vector3 PositionLast = positionLast;
        [FieldOffset(12)] public Vector3 PositionCurrent = positionCurrent;
        [FieldOffset(24)] public int FrameIndex = frameIndex;
        [FieldOffset(28)] public Vector2i FrameSize = frameSize;
        [FieldOffset(36)] public int FrameCount = frameCount;
        [FieldOffset(40)] public int FrameTime = frameTime;
    }

    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct UniformData(Matrix4 projection, Vector2i timeMilliseconds, float alpha, Vector2i textureSize, Vector2i textureCell, int texturePadding) {
        [FieldOffset(0)] public Matrix4 Projection = projection;
        [FieldOffset(64)] public Vector2i TimeMilliseconds = timeMilliseconds;
        [FieldOffset(72)] public float Alpha = alpha;
        [FieldOffset(80)] public Vector2i TextureSize = textureSize;
        [FieldOffset(88)] public Vector2i TextureCell = textureCell;
        [FieldOffset(96)] public int TexturePadding = texturePadding;
    }
}