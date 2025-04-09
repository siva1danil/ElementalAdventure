using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;

namespace ElementalAdventure.Client.Core.Resources;

public class ResourceRegistry : IDisposable {
    private readonly Dictionary<string, ShaderProgram> _shaders;
    private readonly Dictionary<string, Texture2D> _textures;
    private readonly Dictionary<string, TextureAtlas<string>> _textureAtlases;

    public ResourceRegistry() {
        _shaders = [];
        _textures = [];
        _textureAtlases = [];
    }

    public ShaderProgram GetShader(string name) => _shaders[name];
    public void AddShader(string name, ShaderProgram shader) => _shaders[name] = shader;

    public Texture2D GetTexture(string name) => _textures[name];
    public void AddTexture(string name, Texture2D texture) => _textures[name] = texture;

    public TextureAtlas<string> GetTextureAtlas(string name) => _textureAtlases[name];
    public void AddTextureAtlas(string name, TextureAtlas<string> textureAtlas) => _textureAtlases[name] = textureAtlas;

    public void Dispose() {
        foreach (var shader in _shaders.Values) shader.Dispose();
        GC.SuppressFinalize(this);
    }
}