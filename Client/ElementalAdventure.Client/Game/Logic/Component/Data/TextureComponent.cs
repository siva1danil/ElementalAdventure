namespace ElementalAdventure.Client.Game.Logic.Component.Data;

public class TextureDataComponent : IDataComponent {
    private string _textureAtlas;
    private string _texture;

    public string TextureAtlas { get => _textureAtlas; set => _textureAtlas = value; }
    public string Texture { get => _texture; set => _texture = value; }

    public TextureDataComponent(string textureAtlas, string texture) {
        _textureAtlas = textureAtlas;
        _texture = texture;
    }
}