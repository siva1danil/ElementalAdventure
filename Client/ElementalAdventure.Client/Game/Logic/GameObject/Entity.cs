using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic.Component.Behaviour;
using ElementalAdventure.Client.Game.Logic.Component.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.GameObject;

// TODO: refactor internal format
public class Entity {
    private readonly AssetManager<string> _assetManager;
    private readonly PositionDataComponent _positionDataComponent;
    private readonly TextureDataComponent _textureDataComponent;
    private readonly LivingDataComponent? _livingDataComponent;
    private readonly IBehavourComponent[] _behaviourComponents;

    public PositionDataComponent PositionDataComponent => _positionDataComponent;
    public TextureDataComponent TextureDataComponent => _textureDataComponent;
    public LivingDataComponent? LivingDataComponent => _livingDataComponent;

    public Entity(AssetManager<string> assetManager, LivingDataComponent? livingDataComponent, IBehavourComponent[] behaviourComponents) {
        _assetManager = assetManager;
        _positionDataComponent = new PositionDataComponent(new Vector2(0.0f, 0.0f));
        _textureDataComponent = new TextureDataComponent(false, "", "");
        _livingDataComponent = livingDataComponent;
        _behaviourComponents = behaviourComponents;
    }

    public void Update(GameWorld world) {
        foreach (IBehavourComponent behaviourComponent in _behaviourComponents)
            behaviourComponent?.Update(world, this);
    }

    public TilemapShaderLayout.GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public TilemapShaderLayout.InstanceData[] GetInstanceData() {
        if (_textureDataComponent.Visible) {
            TextureAtlas<string> atlas = _assetManager.Get<TextureAtlas<string>>(_textureDataComponent.TextureAtlas);
            TextureAtlas<string>.Entry entry = atlas.GetEntry(_textureDataComponent.Texture);
            return [new(new(_positionDataComponent.LastPosition.X, _positionDataComponent.LastPosition.Y, _positionDataComponent.Z), new(_positionDataComponent.Position.X, _positionDataComponent.Position.Y, _positionDataComponent.Z), entry.Index, new(entry.Width, entry.Height), entry.FrameCount, entry.FrameTime)];
        } else {
            return [];
        }
    }
}