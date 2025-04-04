using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.OpenGL;

public class ShaderProgram : IDisposable {
    private readonly int _id = -1;

    public int Id => _id;

    public ShaderProgram(string vert, string frag) {
        int vertShader = -1, fragShader = -1;
        try {
            vertShader = CompileShader(vert, ShaderType.VertexShader);
            fragShader = CompileShader(frag, ShaderType.FragmentShader);
            _id = LinkProgram(vertShader, fragShader);
        } catch {
            if (vertShader != -1) GL.DeleteShader(vertShader);
            if (fragShader != -1) GL.DeleteShader(fragShader);
            throw;
        }
        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);
    }

    public void Dispose() {
        GL.DeleteProgram(_id);
        GC.SuppressFinalize(this);
    }

    private static int CompileShader(string source, ShaderType type) {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        return success == 0 ? throw new Exception(GL.GetShaderInfoLog(shader)) : shader;
    }

    private static int LinkProgram(int vert, int frag) {
        int program = GL.CreateProgram();
        GL.AttachShader(program, vert);
        GL.AttachShader(program, frag);
        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        return success == 0 ? throw new Exception(GL.GetProgramInfoLog(program)) : program;
    }
}