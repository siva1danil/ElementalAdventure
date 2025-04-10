using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;

namespace ElementalAdventure.Client.Game.Assets;

public class AssetManager : IDisposable {
    private readonly Dictionary<string, ShaderProgram> _shaders;
    private readonly Dictionary<string, Texture2D> _textures;
    private readonly Dictionary<string, TextureAtlas<string>> _textureAtlases;

    private readonly Dictionary<string, TileType> _tileTypes;
    private readonly Dictionary<string, EntityType> _entityTypes;

    public AssetManager() {
        _shaders = [];
        _textures = [];
        _textureAtlases = [];

        _tileTypes = [];
        _entityTypes = [];
    }

    public ShaderProgram GetShader(string name) => _shaders[name];
    public void AddShader(string name, ShaderProgram shader) => _shaders[name] = shader;

    public Texture2D GetTexture(string name) => _textures[name];
    public void AddTexture(string name, Texture2D texture) => _textures[name] = texture;

    public TextureAtlas<string> GetTextureAtlas(string name) => _textureAtlases[name];
    public void AddTextureAtlas(string name, TextureAtlas<string> textureAtlas) => _textureAtlases[name] = textureAtlas;

    public TileType GetTileType(string name) => _tileTypes[name];
    public void AddTileType(string name, TileType tileType) => _tileTypes[name] = tileType;

    public EntityType GetEntityType(string name) => _entityTypes[name];
    public void AddEntityType(string name, EntityType entityType) => _entityTypes[name] = entityType;

    public void Dispose() {
        foreach (ShaderProgram shader in _shaders.Values) shader.Dispose();
        foreach (Texture2D texture in _textures.Values) texture.Dispose();
        foreach (TextureAtlas<string> textureAtlas in _textureAtlases.Values) textureAtlas.Dispose();
        GC.SuppressFinalize(this);
    }
}