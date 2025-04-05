using System.Numerics;
using System.Runtime.InteropServices;

using ElementalAdventure.Client.OpenGL;
using ElementalAdventure.Client.Resources;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Scenes;

public class ExampleScene : IScene {
    private readonly ResourceRegistry _resourceRegistry;

    private readonly VertexArray<Vertex> _vao;

    public ExampleScene(ResourceRegistry resourceRegistry) {
        _resourceRegistry = resourceRegistry;

        _vao = new VertexArray<Vertex>();
        _vao.SetData([new(new(-1, -1, 0)), new(new(1, -1, 0)), new(new(1, 1, 0)), new(new(-1, 1, 0))]);
    }

    public void Update() {
        //
    }

    public void Render() {
        GL.UseProgram(_resourceRegistry.GetShader("default").Id);
        GL.BindVertexArray(_vao.Id);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        _vao.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex(Vector3 position) {
        public Vector3 Position = position;
    }
}