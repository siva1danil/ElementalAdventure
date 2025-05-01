using ElementalAdventure.Client.Core.Resources.Data;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.Resources.OpenGL;

public class VertexArrayInstanced : IDisposable {
    private readonly int _vao;
    private readonly int _vboGlobal, _vboInstance;

    private readonly DataLayout _layout;

    public int Id => _vao;
    public int VertexDataSize => _layout.VertexDataSize;
    public int InstanceDataSize => _layout.InstanceDataSize;

    public VertexArrayInstanced(DataLayout layout) {
        _layout = layout;

        _vao = GL.GenVertexArray();
        _vboGlobal = GL.GenBuffer();
        _vboInstance = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        int index = 0;
        foreach (DataLayout.Entry entry in layout.VertexData) {
            GL.VertexAttribPointer(index, entry.Size, VertexAttribPointerType.Float, false, _layout.VertexDataSize, entry.Offset);
            GL.EnableVertexAttribArray(index);
            index++;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboInstance);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        foreach (DataLayout.Entry entry in layout.InstanceData) {
            GL.VertexAttribPointer(index, entry.Size, VertexAttribPointerType.Float, false, _layout.InstanceDataSize, entry.Offset);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribDivisor(index, 1);
            index++;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void SetGlobalData(byte[] data, BufferUsageHint usage = BufferUsageHint.StaticDraw, int length = -1) {
        if (length == -1)
            length = data.Length;
        if (length % _layout.VertexDataSize != 0)
            throw new ArgumentException($"Data length {data.Length} is not a multiple of stride {_layout.VertexDataSize}.");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, length, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetInstanceData(byte[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw, int length = -1) {
        if (length == -1)
            length = data.Length;
        if (length % _layout.InstanceDataSize != 0)
            throw new ArgumentException($"Data length {data.Length} is not a multiple of stride {_layout.InstanceDataSize}.");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboInstance);
        GL.BufferData(BufferTarget.ArrayBuffer, length, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vboGlobal);
        GL.DeleteBuffer(_vboInstance);
        GC.SuppressFinalize(this);
    }
}