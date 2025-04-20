namespace ElementalAdventure.Client.Game.Logic.Component.Data;

public class TextureDataComponent : IDataComponent {
    private bool _visible;
    private string _textureAtlas;
    private string _texture;

    public bool Visible { get => _visible; set => _visible = value; }
    public string TextureAtlas { get => _textureAtlas; set => _textureAtlas = value; }
    public string Texture { get => _texture; set => _texture = value; }

    public TextureDataComponent(bool visible, string textureAtlas, string texture) {
        _visible = visible;
        _textureAtlas = textureAtlas;
        _texture = texture;
    }
}