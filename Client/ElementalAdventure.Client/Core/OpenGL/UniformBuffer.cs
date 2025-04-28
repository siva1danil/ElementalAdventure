using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.OpenGL;

public class UniformBuffer : IDisposable {
    private readonly int _id;
    private readonly int _size;

    public int Id => _id;
    public int Size => _size;

    public UniformBuffer(Type type) {
        _id = GL.GenBuffer();
        _size = Marshal.SizeOf(type);
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferData(BufferTarget.UniformBuffer, Marshal.SizeOf(type), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void SetData(byte[] data) {
        if (data.Length != _size)
            throw new ArgumentException($"Data length {data.Length} does not match buffer size {_size}.");
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferSubData(BufferTarget.UniformBuffer, 0, data.Length, data);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteBuffer(_id);
        GC.SuppressFinalize(this);
    }
}