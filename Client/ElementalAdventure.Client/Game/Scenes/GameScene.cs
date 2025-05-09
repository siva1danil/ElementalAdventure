using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Rendering;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic;
using ElementalAdventure.Client.Game.Logic.Command;
using ElementalAdventure.Client.Game.Logic.Component.Behaviour;
using ElementalAdventure.Client.Game.Logic.Component.Data;
using ElementalAdventure.Client.Game.Logic.GameObject;
using ElementalAdventure.Client.Game.UI;
using ElementalAdventure.Client.Game.UI.Interface;
using ElementalAdventure.Client.Game.UI.View;
using ElementalAdventure.Client.Game.UI.ViewGroup;

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
        _ui = new(new Vector2(0.0f, 1.0f));
        _uiCamera = new Camera(context.WindowSize / 2.0f, context.WindowSize, context.WindowSize);
        _worldCamera = new Camera(new Vector2(13.0f * 0.5f - 0.5f, 9.0f * 0.5f - 0.5f), new Vector2(14.0f, 10f), context.WindowSize);

        AbsoluteLayout layout = new();
        LinearLayout bottomLeft = new();
        LinearLayout bottomRight = new();
        ImageView wasd = new ImageView() { Size = new Vector2(128f, 128f), Source = new ImageView.ImageSource(new("textureatlas.ui"), _context.AssetManager.Get<TextureAtlas>(new AssetID("textureatlas.ui")).GetEntry(new AssetID("button_wasd_normal"))) };
        ImageView e = new ImageView() { Size = new Vector2(128f, 128f), Source = new ImageView.ImageSource(new("textureatlas.ui"), _context.AssetManager.Get<TextureAtlas>(new AssetID("textureatlas.ui")).GetEntry(new AssetID("button_e_normal"))) };
        ImageView q = new ImageView() { Size = new Vector2(128f, 128f), Source = new ImageView.ImageSource(new("textureatlas.ui"), _context.AssetManager.Get<TextureAtlas>(new AssetID("textureatlas.ui")).GetEntry(new AssetID("button_q_normal"))) };
        ColorView test = new ColorView() { Size = new Vector2(128f, 128f), Color = new Vector3(1.0f, 0.0f, 0.0f) };
        layout.Add(bottomLeft, new AbsoluteLayout.LayoutParams { Position = new Vector2(0.0f, 0.0f), Anchor = new Vector2(0.0f, 0.0f) });
        layout.Add(bottomRight, new AbsoluteLayout.LayoutParams { Position = new Vector2(1280.0f, 0.0f), Anchor = new Vector2(1.0f, 0.0f) });
        bottomLeft.Add(wasd, new LinearLayout.LayoutParams { });
        bottomRight.Add(e, new LinearLayout.LayoutParams { });
        bottomRight.Add(q, new LinearLayout.LayoutParams { });
        bottomRight.Add(test, new LinearLayout.LayoutParams { });
        _ui.Push(layout);

        _world = new GameWorld(1.0f / 20.0f, new Tilemap(), []);
        _world.Tilemap.SetMap(new Vector2(-1.0f, 0.0f), _context.AssetManager, new AssetID[,,] {
            {
                { new("floor_1_righthalf"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("wall_top"), new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") },
                { new("floor_1_righthalf"), new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1"),  new("floor_1_lefthalf") }
            },
            {
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, new("fountain_1_bottom"), AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, new("fountain_2_bottom"), AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,             AssetID.None, AssetID.None, AssetID.None }
            },
            {
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, new("fountain_1_top"), AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, new("fountain_2_top"), AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None },
                { AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None, AssetID.None,          AssetID.None, AssetID.None, AssetID.None }
            },
            {
                { new("wall_topleft_outer"),    AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_topright_outer")    },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_left"),             AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       AssetID.None,       new("wall_right")             },
                { new("wall_bottomleft_outer"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottom"), new("wall_bottomright_outer") }
            }
        }, 1);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(true, false, _context.AssetManager.Get<PlayerType>(new AssetID("mage")).Speed), [new PlayerBehaviourComponent(_context.AssetManager.Get<PlayerType>(new AssetID("mage")))]));
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>(new AssetID("slime")).Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>(new AssetID("slime")))]));
        _world.Entities[1].PositionDataComponent.Position = new Vector2(5.0f, 0.0f);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>(new AssetID("slime")).Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>(new AssetID("slime")))]));
        _world.Entities[2].PositionDataComponent.Position = new Vector2(15.0f, 4.0f);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>(new AssetID("slime")).Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>(new AssetID("slime")))]));
        _world.Entities[3].PositionDataComponent.Position = new Vector2(5.0f, 4.0f);

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
        _worldCamera.ScreenSize = new Vector2(args.Size.X, args.Size.Y);
    }

    public void KeyDown(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void KeyUp(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void GetUniformData(AssetID shaderProgram, AssetID textureAtlas, Span<byte> buffer) {
        if (shaderProgram == new AssetID("shader.userinterface")) {
            if (textureAtlas != AssetID.None) {
                TextureAtlas atlas = _context.AssetManager.Get<TextureAtlas>(textureAtlas);
                UserInterfaceShaderLayout.UniformData data = new UserInterfaceShaderLayout.UniformData(
                    _uiCamera.GetViewMatrix(),
                    new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                    new Vector2i(atlas.AtlasWidth, atlas.AtlasHeight),
                    new Vector2i(atlas.CellWidth, atlas.CellHeight),
                    atlas.CellPadding
                );
                MemoryMarshal.Cast<UserInterfaceShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).CopyTo(buffer);
            } else {
                UserInterfaceShaderLayout.UniformData data = new UserInterfaceShaderLayout.UniformData(
                    _uiCamera.GetViewMatrix(),
                    new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF))
                );
                MemoryMarshal.Cast<UserInterfaceShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).CopyTo(buffer);
            }
        } else if (shaderProgram == new AssetID("shader.tilemap")) {
            TextureAtlas atlas = _context.AssetManager.Get<TextureAtlas>(textureAtlas);
            TilemapShaderLayout.UniformData data = new TilemapShaderLayout.UniformData(
                _worldCamera.GetViewMatrix(),
                new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
                new Vector2i(atlas.AtlasWidth, atlas.AtlasHeight),
                new Vector2i(atlas.CellWidth, atlas.CellHeight),
                atlas.CellPadding
            );
            MemoryMarshal.Cast<TilemapShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).CopyTo(buffer);
        }
    }

    public void Dispose() {
        _renderer.Dispose();
    }
}