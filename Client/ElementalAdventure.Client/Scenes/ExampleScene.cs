using System.Runtime.InteropServices;

using ElementalAdventure.Client.OpenGL;
using ElementalAdventure.Client.Resources;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Scenes;

public class ExampleScene : IScene {
    private readonly ResourceRegistry _resourceRegistry;

    private readonly VertexArray<Vertex> _vao;
    private readonly StorageBuffer<Instance> _ssbo;
    private Matrix4 _projection;

    public ExampleScene(ResourceRegistry resourceRegistry) {
        _resourceRegistry = resourceRegistry;

        _vao = new VertexArray<Vertex>();
        _vao.SetData([new(new(0, 0, 0)), new(new(1, 0, 0)), new(new(0, 1, 0)), new(new(1, 0, 0)), new(new(0, 1, 0)), new(new(1, 1, 0))]);

        _ssbo = new StorageBuffer<Instance>();
        _ssbo.SetData([new(new(0, 0, 0), new(1, 0, 0)), new(new(1, 0, 0), new(0, 1, 0)), new(new(0, 1, 0), new(0, 1, 0)), new(new(1, 1, 0), new(1, 0, 0)), new(new(2, 2, 0), new(1, 1, 1))]);
    }

    public void Update() {
        _projection = Matrix4.CreateOrthographicOffCenter(-3, 3, -3, 3, -1, 1); // TODO: Move to resize event, dirtyness check
    }

    public void Render() {
        GL.UseProgram(_resourceRegistry.GetShader("default").Id);
        GL.BindVertexArray(_vao.Id);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _ssbo.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _resourceRegistry.GetTexture("default").Id);

        GL.UniformMatrix4(GL.GetUniformLocation(_resourceRegistry.GetShader("default").Id, "uProjection"), false, ref _projection);
        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 5);

        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        _vao.Dispose();
    }

    [StructLayout(LayoutKind.Explicit, Size = 12)]
    private struct Vertex(Vector3 position) {
        [FieldOffset(0)] public Vector3 Position = position;
    }

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    private struct Instance(Vector3 position, Vector3 color) {
        [FieldOffset(0)] public Vector3 Position = position;
        [FieldOffset(16)] public Vector3 Color = color;
    }
}