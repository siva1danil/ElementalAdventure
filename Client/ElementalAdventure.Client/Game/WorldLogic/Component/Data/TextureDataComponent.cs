using ElementalAdventure.Client.Core.Assets;

namespace ElementalAdventure.Client.Game.WorldLogic.Component.Data;

public class TextureDataComponent : IDataComponent {
    private bool _visible;
    private AssetID _textureAtlas;
    private AssetID _texture;

    public bool Visible { get => _visible; set => _visible = value; }
    public AssetID TextureAtlas { get => _textureAtlas; set => _textureAtlas = value; }
    public AssetID Texture { get => _texture; set => _texture = value; }

    public TextureDataComponent(bool visible, AssetID textureAtlas, AssetID texture) {
        _visible = visible;
        _textureAtlas = textureAtlas;
        _texture = texture;
    }
}