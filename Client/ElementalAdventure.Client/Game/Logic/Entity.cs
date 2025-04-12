using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Assets;
using ElementalAdventure.Client.Game.Data;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Logic;

public class Entity {
    private readonly AssetManager _assetManager;
    private readonly EntityType _entityType;

    private Vector3 _positionLast, _positionCurrent, _velocity;

    public Vector3 PositionLast { get => _positionCurrent; set => _positionCurrent = value; }
    public Vector3 PositionCurrent { get => _positionCurrent; set => _positionCurrent = value; }
    public Vector3 Velocity { get => _velocity; set => _velocity = value; }

    public Entity(AssetManager assetManager, EntityType entityType, Vector3 position) {
        _assetManager = assetManager;
        _entityType = entityType;
        _positionLast = _positionCurrent = position;
        _velocity = Vector3.Zero;
    }

    public TilemapShaderLayout.GlobalData[] GetGlobalData() {
        return [new(new(0.0f, 0.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 0.0f, 0.0f)), new(new(0.0f, 1.0f, 0.0f)), new(new(1.0f, 1.0f, 0.0f))];
    }

    public TilemapShaderLayout.InstanceData[] GetInstanceData() {
        TextureAtlas<string> atlas = _assetManager.GetTextureAtlas(_entityType.TextureAtlas);
        TextureAtlas<string>.Entry entry = atlas.GetEntry(_entityType.Texture);
        return [new(_positionCurrent, entry.Index, entry.FrameCount, entry.FrameTime)];
    }
}