using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Assets;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic.Component;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic.GameObject;

public class Entity {
    private readonly AssetManager _assetManager;
    private readonly EntityType _entityType;

    private readonly PositionComponent _positionComponent;
    private readonly PlayerControllableComponent? _playerControllableComponent;


    public EntityType EntityType => _entityType;

    public PositionComponent PositionComponent => _positionComponent;
    public PlayerControllableComponent? PlayerControllableComponent => _playerControllableComponent;

    public Entity(AssetManager assetManager, EntityType entityType, Vector2 position, bool controllable) {
        _assetManager = assetManager;
        _entityType = entityType;

        _positionComponent = new PositionComponent(position);
        _playerControllableComponent = controllable ? new PlayerControllableComponent() : null;

        _positionComponent.Z = 1.0f; // TODO: remove hardcode
    }

    public TilemapShaderLayout.GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public TilemapShaderLayout.InstanceData[] GetInstanceData() {
        TextureAtlas<string> atlas = _assetManager.GetTextureAtlas(_entityType.TextureAtlas);
        TextureAtlas<string>.Entry entry = atlas.GetEntry(_entityType.TextureIdleLeft);
        return [new(new(_positionComponent.LastPosition.X, _positionComponent.LastPosition.Y, _positionComponent.Z), new(_positionComponent.Position.X, _positionComponent.Position.Y, _positionComponent.Z), entry.Index, entry.FrameCount, entry.FrameTime)];
    }
}