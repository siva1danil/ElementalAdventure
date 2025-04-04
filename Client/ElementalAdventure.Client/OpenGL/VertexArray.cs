using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.OpenGL;

public class VertexArray : IDisposable {
    private readonly int _vao = -1;
    private readonly int _vbo = -1;

    public int Id => _vao;

    public VertexArray(int[] attributes) {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        int stride = 0;
        foreach (int attribute in attributes)
            stride += attribute * sizeof(float);

        int offset = 0;
        for (int i = 0; i < attributes.Length; i++) {
            GL.EnableVertexAttribArray(i);
            GL.VertexAttribPointer(i, attributes[i], VertexAttribPointerType.Float, false, stride, offset);
            offset += attributes[i] * sizeof(float);
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void SetData(float[] data) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GC.SuppressFinalize(this);
    }
}