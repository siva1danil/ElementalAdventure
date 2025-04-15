using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Assets;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic.Component;
using ElementalAdventure.Client.Game.Logic.Component.Behaviour;
using ElementalAdventure.Client.Game.Logic.Component.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.GameObject;

// TODO: refactor internal format
public class Entity {
    private readonly AssetManager _assetManager;
    private readonly EntityType _entityType;

    private readonly PositionDataComponent _positionDataComponent;
    private readonly TextureDataComponent _textureDataComponent;
    private readonly ControllableBehaviourComponent? _controllableBehaviourComponent;
    private readonly MovingBehaviourComponent _movingBehaviourComponentl;

    public EntityType EntityType => _entityType;

    public PositionDataComponent PositionDataComponent => _positionDataComponent;
    public TextureDataComponent TextureDataComponent => _textureDataComponent;
    public ControllableBehaviourComponent? ControllableBehaviourComponent => _controllableBehaviourComponent;
    public MovingBehaviourComponent? MovingBehaviourComponent => _movingBehaviourComponentl;

    public Entity(AssetManager assetManager, EntityType entityType, Vector2 position, bool controllable) {
        _assetManager = assetManager;
        _entityType = entityType;

        _positionDataComponent = new PositionDataComponent(position);
        _textureDataComponent = new TextureDataComponent(entityType.TextureAtlas, entityType.TextureIdleLeft);
        _controllableBehaviourComponent = controllable ? new ControllableBehaviourComponent() : null;
        _movingBehaviourComponentl = new MovingBehaviourComponent();

        _positionDataComponent.Z = 1.0f; // TODO: remove hardcode
    }

    public TilemapShaderLayout.GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public TilemapShaderLayout.InstanceData[] GetInstanceData() {
        TextureAtlas<string> atlas = _assetManager.GetTextureAtlas(_entityType.TextureAtlas);
        TextureAtlas<string>.Entry entry = atlas.GetEntry(_entityType.TextureIdleLeft);
        return [new(new(_positionDataComponent.LastPosition.X, _positionDataComponent.LastPosition.Y, _positionDataComponent.Z), new(_positionDataComponent.Position.X, _positionDataComponent.Position.Y, _positionDataComponent.Z), entry.Index, entry.FrameCount, entry.FrameTime)];
    }
}