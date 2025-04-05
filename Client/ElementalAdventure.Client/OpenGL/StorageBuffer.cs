using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.OpenGL;

public class StorageBuffer<T> : IDisposable where T : struct {
    private readonly int _id;

    private readonly int _stride;

    public int Id => _id;

    public StorageBuffer() {
        _id = GL.GenBuffer();
        _stride = Marshal.SizeOf<T>();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _id);
        GL.BufferData(BufferTarget.ShaderStorageBuffer, 0, IntPtr.Zero, BufferUsageHint.StreamDraw);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void SetData(T[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _id);
        GL.BufferData(BufferTarget.ShaderStorageBuffer, data.Length * _stride, data, usage);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteBuffer(_id);
        GC.SuppressFinalize(this);
    }
}
