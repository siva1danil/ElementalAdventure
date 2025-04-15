using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.OpenGL;

public class UniformBuffer<T> : IDisposable where T : unmanaged {
    private readonly int _id;

    public int Id => _id;

    public UniformBuffer() {
        _id = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferData(BufferTarget.UniformBuffer, Marshal.SizeOf<T>(), IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void SetData(ref T data) {
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferSubData(BufferTarget.UniformBuffer, 0, Marshal.SizeOf<T>(), ref data);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteProgram(_id);
        GC.SuppressFinalize(this);
    }
}