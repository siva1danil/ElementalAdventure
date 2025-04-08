using ElementalAdventure.Client.Core.OpenGL;

namespace ElementalAdventure.Client.Core.Resources;

public class ResourceRegistry : IDisposable {
    private readonly Dictionary<string, ShaderProgram> _shaders;
    private readonly Dictionary<string, Texture2D> _textures;

    public ResourceRegistry() {
        _shaders = [];
        _textures = [];
    }

    public ShaderProgram GetShader(string name) => _shaders[name];
    public void AddShader(string name, ShaderProgram shader) => _shaders[name] = shader;

    public Texture2D GetTexture(string name) => _textures[name];
    public void AddTexture(string name, Texture2D texture) => _textures[name] = texture;

    public void Dispose() {
        foreach (var shader in _shaders.Values) shader.Dispose();
        GC.SuppressFinalize(this);
    }
}