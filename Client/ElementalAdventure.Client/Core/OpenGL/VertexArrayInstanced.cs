using System.Reflection;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.OpenGL;

public class VertexArrayInstanced : IDisposable {
    private readonly int _vao;
    private readonly int _vboGlobal, _vboInstance;

    private readonly int _strideGlobal, _strideInstance;

    public int Id => _vao;
    public int GlobalStride => _strideGlobal;
    public int InstanceStride => _strideInstance;

    public VertexArrayInstanced(Type globalType, Type instanceType) {
        _vao = GL.GenVertexArray();
        _vboGlobal = GL.GenBuffer();
        _vboInstance = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _strideGlobal = Marshal.SizeOf(globalType);
        int index = 0;
        foreach (FieldInfo field in globalType.GetFields()) {
            int size = Marshal.SizeOf(field.FieldType) / sizeof(float);
            int offset = Marshal.OffsetOf(globalType, field.Name).ToInt32();
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, _strideGlobal, offset);
            GL.EnableVertexAttribArray(index);
            index++;
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboInstance);
        GL.BufferData(BufferTarget.ArrayBuffer, 0, IntPtr.Zero, BufferUsageHint.DynamicDraw);

        _strideInstance = Marshal.SizeOf(instanceType);
        foreach (FieldInfo field in instanceType.GetFields()) {
            int size = Marshal.SizeOf(field.FieldType) / sizeof(float);
            int offset = Marshal.OffsetOf(instanceType, field.Name).ToInt32();
            GL.VertexAttribPointer(index, size, VertexAttribPointerType.Float, false, _strideInstance, offset);
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
        if (length % _strideGlobal != 0)
            throw new ArgumentException($"Data length {data.Length} is not a multiple of stride {_strideGlobal}.");
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboGlobal);
        GL.BufferData(BufferTarget.ArrayBuffer, length, data, usage);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetInstanceData(byte[] data, BufferUsageHint usage = BufferUsageHint.DynamicDraw, int length = -1) {
        if (length == -1)
            length = data.Length;
        if (length % _strideInstance != 0)
            throw new ArgumentException($"Data length {data.Length} is not a multiple of stride {_strideInstance}.");
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