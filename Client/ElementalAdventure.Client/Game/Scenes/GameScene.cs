using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.HighLevel;
using ElementalAdventure.Client.Core.UI;
using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.Components.UI.ViewGroups;
using ElementalAdventure.Client.Game.Components.UI.Views;
using ElementalAdventure.Client.Game.Components.Utils;
using ElementalAdventure.Client.Game.SystemLogic;
using ElementalAdventure.Client.Game.WorldLogic;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Client.Game.WorldLogic.Component.Behaviour;
using ElementalAdventure.Client.Game.WorldLogic.Component.Data;
using ElementalAdventure.Client.Game.WorldLogic.GameObject;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene, IUniformProvider {
    private readonly ClientContext _context;

    private readonly BatchedRenderer _renderer;
    private readonly UIManager _ui;
    private readonly Camera _worldCamera, _uiCamera;
    private readonly GameWorld _world;

    private double _tickAccumulator;

    public GameScene(ClientContext context) {
        _context = context;

        _renderer = new(_context.AssetManager, this);
        _ui = new(new Vector2(0.0f, 1.0f), _context.WindowSize);
        _uiCamera = new Camera(context.WindowSize / 2.0f, context.WindowSize, context.WindowSize, true);
        _worldCamera = new Camera(new Vector2(5.0f, 5.0f), new Vector2(8.0f, 8.0f), context.WindowSize);

        AbsoluteLayout layout = new();
        LinearLayout bottomLeft = new();
        LinearLayout bottomRight = new();
        ImageView wasd = new ImageView(_context.AssetManager) { Size = new Vector2(0f, 64f), AspectRatio = ImageView.AspectRatioType.AdjustWidth, ImageTextureAtlas = new AssetID("textureatlas.ui"), ImageTextureEntry = new AssetID("button_wasd_normal") };
        ImageView e = new ImageView(_context.AssetManager) { Size = new Vector2(0f, 32f), AspectRatio = ImageView.AspectRatioType.AdjustWidth, ImageTextureAtlas = new AssetID("textureatlas.ui"), ImageTextureEntry = new AssetID("button_e_normal") };
        ImageView q = new ImageView(_context.AssetManager) { Size = new Vector2(0f, 32f), AspectRatio = ImageView.AspectRatioType.AdjustWidth, ImageTextureAtlas = new AssetID("textureatlas.ui"), ImageTextureEntry = new AssetID("button_q_normal") };
        layout.Add(bottomLeft, new AbsoluteLayout.LayoutParams { Position = new Vector2(0.0f, 1.0f), Anchor = new Vector2(0.0f, 1.0f) });
        layout.Add(bottomRight, new AbsoluteLayout.LayoutParams { Position = new Vector2(1.0f, 1.0f), Anchor = new Vector2(1.0f, 1.0f) });
        bottomLeft.Add(wasd, new LinearLayout.LayoutParams { Margin = new Vector4(16.0f, 16.0f, 16.0f, 16.0f) });
        bottomRight.Add(e, new LinearLayout.LayoutParams { Margin = new Vector4(16.0f, 16.0f, 16.0f, 16.0f) });
        bottomRight.Add(q, new LinearLayout.LayoutParams { Margin = new Vector4(16.0f, 16.0f, 16.0f, 0.0f) });
        _ui.Push(layout);

        _world = new GameWorld(1.0f / 20.0f, new Tilemap(), []);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(true, false, _context.AssetManager.Get<PlayerType>(new AssetID("mage")).Speed), new HitboxDataComponent(new Box2(-0.01f, -0.5f + 2.0f / 32.0f - 0.01f, 0.01f, -0.5f + 2.0f / 32.0f + 0.01f)), [new PlayerBehaviourComponent(_context.AssetManager.Get<PlayerType>(new AssetID("mage")))]));
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>(new AssetID("slime")).Speed), new HitboxDataComponent(new Box2(-0.01f, -0.5f + 2.0f / 32.0f - 0.01f, 0.01f, -0.5f + 2.0f / 32.0f + 0.01f)), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>(new AssetID("slime")))]));
        _world.Entities[0].PositionDataComponent.Position = new Vector2(0.0f, 0.0f);
        _world.Entities[1].PositionDataComponent.Position = new Vector2(5.0f, 0.0f);

        _tickAccumulator = 0.0;
    }

    public void Update(FrameEventArgs args) {
        _tickAccumulator += args.Time;
        while (_tickAccumulator >= _world.TickInterval) {
            _tickAccumulator -= _world.TickInterval;
            _world.Tick();
        }
    }

    public void Render(FrameEventArgs args) {
        _world.Tilemap.Render(_renderer);
        foreach (Entity entity in _world.Entities)
            entity.Render(_renderer);
        _ui.Render(_renderer);
        _renderer.Commit();
        _renderer.Render();
    }

    public void Resize(ResizeEventArgs args) {
        _uiCamera.Center = new Vector2(args.Size.X, args.Size.Y) / 2.0f;
        _uiCamera.ScreenSize = new Vector2(args.Size.X, args.Size.Y);
        _uiCamera.TargetWorldSize = new Vector2(args.Size.X, args.Size.Y);
        _ui.Size = new Vector2(args.Size.X, args.Size.Y);
        _worldCamera.ScreenSize = new Vector2(args.Size.X, args.Size.Y);
    }

    public void KeyDown(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void KeyUp(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void SetTilemap(AssetID[,,] tilemap, Box2[] walls, int midground) {
        _world.Tilemap.SetMap(new Vector2(-1.0f, 0.0f), _context.AssetManager, tilemap, midground);
        _world.Tilemap.SetWalls(walls);
        _worldCamera.Center = new Vector2(tilemap.GetLength(2) * 0.5f - 0.5f, tilemap.GetLength(1) * 0.5f - 0.5f);
        _worldCamera.TargetWorldSize = new Vector2(tilemap.GetLength(2), tilemap.GetLength(1));
    }

    public void SetPlayerPosition(Vector2 position) {
        foreach (Entity entity in _world.Entities) {
            if (entity.Has<PlayerBehaviourComponent>())
                entity.PositionDataComponent.Position = position;
        }
    }

    public void GetUniformData(AssetID shaderProgram, AssetID textureAtlas, Span<byte> buffer) {
        Vector2i time = new((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF));
        if (shaderProgram == new AssetID("shader.userinterface")) {
            if (textureAtlas == AssetID.None) {
                UserInterfaceShaderLayout.UniformData data = new(_uiCamera.GetViewMatrix(), time);
                MemoryMarshal.Write(buffer, data);
            } else {
                TextureAtlas atlas = _context.AssetManager.Get<TextureAtlas>(textureAtlas);
                UserInterfaceShaderLayout.UniformData data = new(_uiCamera.GetViewMatrix(), time, new(atlas.AtlasWidth, atlas.AtlasHeight), new(atlas.CellWidth, atlas.CellHeight), atlas.CellPadding);
                MemoryMarshal.Write(buffer, data);
            }
        } else if (shaderProgram == new AssetID("shader.tilemap")) {
            float alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f;
            TextureAtlas atlas = _context.AssetManager.Get<TextureAtlas>(textureAtlas);
            TilemapShaderLayout.UniformData data = new TilemapShaderLayout.UniformData(_worldCamera.GetViewMatrix(), time, alpha, new(atlas.AtlasWidth, atlas.AtlasHeight), new(atlas.CellWidth, atlas.CellHeight), atlas.CellPadding);
            MemoryMarshal.Write(buffer, data);
        } else {
            throw new NotImplementedException($"Shader program {shaderProgram} not supported in {nameof(GameScene)}.");
        }
    }

    public void Dispose() {
        _renderer.Dispose();
    }
}