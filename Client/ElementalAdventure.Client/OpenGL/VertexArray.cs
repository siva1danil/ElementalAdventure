using System.Reflection;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.OpenGL;

public class VertexArray<T> : IDisposable where T : struct {
    private readonly int _vao = -1;
    private readonly int _vbo = -1;

    private readonly int _stride;

    public int Id => _vao;

    public VertexArray() {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _stride = Marshal.SizeOf<T>();
        int index = 0;
        foreach (FieldInfo field in typeof(T).GetFields()) {
            int size = Marshal.SizeOf(field.FieldType) / sizeof(float);
            int offset = Marshal.OffsetOf<T>(field.Name).ToInt32();
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, _stride, offset);
            GL.EnableVertexAttribArray(index);
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void SetData(T[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * _stride, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vbo);
        GC.SuppressFinalize(this);
    }
}