using ElementalAdventure.Client.OpenGL;
using ElementalAdventure.Client.Resources;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Scenes;

public class ExampleScene : IScene {
    private readonly ResourceRegistry _resourceRegistry;

    private readonly VertexArray _vao;

    public ExampleScene(ResourceRegistry resourceRegistry) {
        _resourceRegistry = resourceRegistry;

        _vao = new VertexArray([3]);
        _vao.SetData([-1, -1, 0, 1, -1, 0, 1, 1, 0, -1, 1, 0]);
    }

    public void Update() {
        //
    }

    public void Render() {
        GL.UseProgram(_resourceRegistry.GetShader("default").Id);
        GL.BindVertexArray(_vao.Id);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 4);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        _vao.Dispose();
    }
}