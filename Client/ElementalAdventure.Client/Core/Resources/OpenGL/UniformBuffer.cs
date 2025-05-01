using ElementalAdventure.Client.Core.Resources.Data;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.Resources.OpenGL;

public class UniformBuffer : IDisposable {
    private readonly int _id;
    private readonly DataLayout _layout;

    public int Id => _id;
    public int Size => _layout.UniformDataSize;

    public UniformBuffer(DataLayout layout) {
        _layout = layout;

        _id = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferData(BufferTarget.UniformBuffer, layout.UniformDataSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void SetData(byte[] data) {
        if (data.Length != _layout.UniformDataSize)
            throw new ArgumentException($"Data length {data.Length} does not match buffer size {_layout.UniformDataSize}.");
        GL.BindBuffer(BufferTarget.UniformBuffer, _id);
        GL.BufferSubData(BufferTarget.UniformBuffer, 0, data.Length, data);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteBuffer(_id);
        GC.SuppressFinalize(this);
    }
}