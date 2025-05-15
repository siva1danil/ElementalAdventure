using System.Runtime.InteropServices;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Data;

public static class UserInterfaceShaderLayout {
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct GlobalData(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 76)]
    public struct InstanceData(Vector3 position, Vector2 scale, Vector3 color, int hasTexture, int frameIndex, Vector2i frameSize, int frameCount, int frameTime, int hasRawUV, Vector4 RawUV) {
        public InstanceData(Vector3 position, Vector2 scale, Vector3 color) : this(position, scale, color, 0, 0, Vector2i.Zero, 0, 1, 0, Vector4.Zero) { }
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(12)] public Vector2 Scale = scale;
        [FieldOffset(20)] public Vector3 Color = color;
        [FieldOffset(32)] public int HasTexture = hasTexture;
        [FieldOffset(36)] public int FrameIndex = frameIndex;
        [FieldOffset(40)] public Vector2i FrameSize = frameSize;
        [FieldOffset(48)] public int FrameCount = frameCount;
        [FieldOffset(52)] public int FrameTime = frameTime;
        [FieldOffset(56)] public int HasRawUV = hasRawUV;
        [FieldOffset(60)] public Vector4 RawUV = RawUV;
    }

    [StructLayout(LayoutKind.Explicit, Size = 96)]
    public struct UniformData(Matrix4 projection, Vector2i timeMilliseconds, Vector2i textureSize, Vector2i textureCell, int texturePadding) {
        public UniformData(Matrix4 projection, Vector2i timeMilliseconds) : this(projection, timeMilliseconds, Vector2i.Zero, Vector2i.Zero, 0) { }
        [FieldOffset(0)] public Matrix4 Projection = projection;
        [FieldOffset(64)] public Vector2i TimeMilliseconds = timeMilliseconds;
        [FieldOffset(72)] public Vector2i TextureSize = textureSize;
        [FieldOffset(80)] public Vector2i TextureCell = textureCell;
        [FieldOffset(88)] public int TexturePadding = texturePadding;
    }
}