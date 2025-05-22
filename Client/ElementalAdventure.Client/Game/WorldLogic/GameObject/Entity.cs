using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;
using ElementalAdventure.Client.Game.WorldLogic.Component.Data;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.WorldLogic.GameObject;

// TODO: refactor internal format
public class Entity {
    private readonly AssetManager _assetManager;
    private readonly PositionDataComponent _positionDataComponent;
    private readonly TextureDataComponent _textureDataComponent;
    private readonly LivingDataComponent? _livingDataComponent;
    private readonly HitboxDataComponent? _hitboxDataComponent;
    private readonly IBehavourComponent[] _behaviourComponents;

    private readonly TilemapShaderLayout.GlobalData[] _globalData;

    public PositionDataComponent PositionDataComponent => _positionDataComponent;
    public TextureDataComponent TextureDataComponent => _textureDataComponent;
    public LivingDataComponent? LivingDataComponent => _livingDataComponent;
    public HitboxDataComponent? HitboxDataComponent => _hitboxDataComponent;

    public Entity(AssetManager assetManager, LivingDataComponent? livingDataComponent, HitboxDataComponent? hitboxDataComponent, IBehavourComponent[] behaviourComponents) {
        _assetManager = assetManager;
        _positionDataComponent = new PositionDataComponent(new Vector2(0.0f, 0.0f));
        _textureDataComponent = new TextureDataComponent(false, AssetID.None, AssetID.None);
        _livingDataComponent = livingDataComponent;
        _hitboxDataComponent = hitboxDataComponent;
        _behaviourComponents = behaviourComponents;

        _globalData = [new(new(-0.5f, -0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, -0.5f, 0.0f)), new(new(-0.5f, 0.5f, 0.0f)), new(new(0.5f, 0.5f, 0.0f))];
    }

    public bool Has<T>() {
        for (int i = 0; i < _behaviourComponents.Length; i++)
            if (_behaviourComponents[i] is T)
                return true;
        return false;
    }

    public void Update(GameWorld world) {
        foreach (IBehavourComponent behaviourComponent in _behaviourComponents)
            behaviourComponent?.Update(world, this);
    }

    public void Render(IRenderer renderer) {
        if (!TextureDataComponent.Visible)
            return;

        Span<byte> slot = renderer.AllocateInstance(this, 0, new AssetID("shader.tilemap"), TextureDataComponent.TextureAtlas, MemoryMarshal.Cast<TilemapShaderLayout.GlobalData, byte>(_globalData.AsSpan()), Marshal.SizeOf<TilemapShaderLayout.InstanceData>());
        TextureAtlas.Entry entry = _assetManager.Get<TextureAtlas>(TextureDataComponent.TextureAtlas).GetEntry(TextureDataComponent.Texture);
        TilemapShaderLayout.InstanceData instance = new(new(PositionDataComponent.LastPosition.X, PositionDataComponent.LastPosition.Y, PositionDataComponent.Z), new(PositionDataComponent.Position.X, PositionDataComponent.Position.Y, PositionDataComponent.Z), entry.Index, new Vector2i(entry.Width, entry.Height), entry.FrameCount, entry.FrameTime);
        MemoryMarshal.Write(slot, instance);
    }
}