using System.Reflection;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.OpenGL;

public class VertexArrayInstanced<T0, T1> : IDisposable where T0 : unmanaged where T1 : unmanaged {
    private readonly int _vao;
    private readonly int _vboGlobal, _vboInstance;

    private readonly int _strideGlobal, _strideInstance;

    public int Id => _vao;

    public VertexArrayInstanced() {
        _vao = GL.GenVertexArray();
        _vboGlobal = GL.GenBuffer();
        _vboInstance = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _strideGlobal = Marshal.SizeOf<T0>();
        int index = 0;
        foreach (FieldInfo field in typeof(T0).GetFields()) {
            int size = Marshal.SizeOf(field.FieldType) / sizeof(float);
            int offset = Marshal.OffsetOf<T0>(field.Name).ToInt32();
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, _strideGlobal, offset);
            GL.EnableVertexAttribArray(index);
            index++;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboInstance);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _strideInstance = Marshal.SizeOf<T1>();
        foreach (FieldInfo field in typeof(T1).GetFields()) {
            int size = Marshal.SizeOf(field.FieldType) / sizeof(float);
            int offset = Marshal.OffsetOf<T1>(field.Name).ToInt32();
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, _strideInstance, offset);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribDivisor(index, 1);
            index++;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void SetGlobalData(T0[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * _strideGlobal, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetInstanceData(T1[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw) {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboInstance);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Length * _strideInstance, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void Dispose() {
        GL.DeleteVertexArray(_vao);
        GL.DeleteBuffer(_vboGlobal);
        GL.DeleteBuffer(_vboInstance);
        GC.SuppressFinalize(this);
    }
}