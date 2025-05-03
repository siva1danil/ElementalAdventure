using ElementalAdventure.Client.Core.Assets;
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

    private readonly TilemapShaderLayout.GlobalData[] _globalData;
    private readonly TilemapShaderLayout.InstanceData[] _instanceData;

    public PositionDataComponent PositionDataComponent => _positionDataComponent;
    public TextureDataComponent TextureDataComponent => _textureDataComponent;
    public LivingDataComponent? LivingDataComponent => _livingDataComponent;

    public Entity(AssetManager<string> assetManager, LivingDataComponent? livingDataComponent, IBehavourComponent[] behaviourComponents) {
        _assetManager = assetManager;
        _positionDataComponent = new PositionDataComponent(new Vector2(0.0f, 0.0f));
        _textureDataComponent = new TextureDataComponent(false, "", "");
        _livingDataComponent = livingDataComponent;
        _behaviourComponents = behaviourComponents;

        _globalData = [new(new(-0.5f, -0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, 0.5f, 0.0f))];
        _instanceData = new TilemapShaderLayout.InstanceData[1];
    }

    public void Update(GameWorld world) {
        foreach (IBehavourComponent behaviourComponent in _behaviourComponents)
            behaviourComponent?.Update(world, this);
    }
}