using ElementalAdventure.Client.OpenGL;

namespace ElementalAdventure.Client.Resources;

public class ResourceRegistry : IDisposable {
    private readonly Dictionary<string, ShaderProgram> _shaders;

    public ResourceRegistry() {
        _shaders = [];
    }

    public ShaderProgram GetShader(string name) => _shaders[name];
    public void AddShader(string name, ShaderProgram shader) => _shaders[name] = shader;

    public void Dispose() {
        foreach (var shader in _shaders.Values) shader.Dispose();
        GC.SuppressFinalize(this);
    }
}