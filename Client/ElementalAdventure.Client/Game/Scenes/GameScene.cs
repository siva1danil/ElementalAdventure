using System.Runtime.InteropServices;

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

public class GameScene : IScene, IUniformProvider<string> {
    private readonly ClientContext _context;

    private readonly BatchedRenderer<string> _renderer;
    private readonly UIManager _ui;
    private readonly Camera _worldCamera, _uiCamera;
    private readonly GameWorld _world;

    private double _tickAccumulator;

    public GameScene(ClientContext context) {
        _context = context;

        _renderer = new(_context.AssetManager, this);
        _ui = new();
        _uiCamera = new Camera(context.WindowSize / 2.0f, context.WindowSize, context.WindowSize);
        _worldCamera = new Camera(new Vector2(13.0f * 0.5f - 0.5f, 9.0f * 0.5f - 0.5f), new Vector2(14.0f, 10f), context.WindowSize);

        AbsoluteLayout<string> layout = new();
        LinearLayout<string> bottomLeft = new();
        LinearLayout<string> bottomRight = new();
        LinearLayout<string> center = new() { Orientation = LinearLayout<string>.OrientationType.Vertical };
        ColorView bottomLeftView1 = new() { Size = new Vector2(100f, 100f), Color = new Vector3(1.0f, 0.0f, 0.0f) };
        ColorView bottomLeftView2 = new() { Size = new Vector2(100f, 100f), Color = new Vector3(0.0f, 1.0f, 0.0f) };
        ColorView bottomRightView1 = new() { Size = new Vector2(80f, 80f), Color = new Vector3(0.0f, 0.0f, 1.0f) };
        ColorView bottomRightView2 = new() { Size = new Vector2(80f, 80f), Color = new Vector3(1.0f, 1.0f, 0.0f) };
        ColorView centerView1 = new() { Size = new Vector2(200f, 200f), Color = new Vector3(0.0f, 1.0f, 1.0f) };
        ColorView centerView2 = new() { Size = new Vector2(200f, 200f), Color = new Vector3(1.0f, 0.0f, 1.0f) };
        layout.Add(bottomLeft, new AbsoluteLayout<string>.LayoutParams { Position = new Vector2(0.0f, 0.0f), Anchor = new Vector2(0.0f, 0.0f) });
        layout.Add(bottomRight, new AbsoluteLayout<string>.LayoutParams { Position = new Vector2(1280.0f, 0.0f), Anchor = new Vector2(1.0f, 0.0f) });
        layout.Add(center, new AbsoluteLayout<string>.LayoutParams { Position = new Vector2(1280.0f, 720f) * 0.5f, Anchor = new Vector2(0.5f, 0.5f) });
        bottomLeft.Add(bottomLeftView1, new LinearLayout<string>.LayoutParams { });
        bottomLeft.Add(bottomLeftView2, new LinearLayout<string>.LayoutParams { });
        bottomRight.Add(bottomRightView1, new LinearLayout<string>.LayoutParams { });
        bottomRight.Add(bottomRightView2, new LinearLayout<string>.LayoutParams { });
        center.Add(centerView1, new LinearLayout<string>.LayoutParams { });
        center.Add(centerView2, new LinearLayout<string>.LayoutParams { });
        layout.Measure();
        layout.Layout();
        _ui.Push(layout);

        _world = new GameWorld(1.0f / 20.0f, new Tilemap(), []);
        _world.Tilemap.SetMap(new Vector2(0.0f, 1.0f), _context.AssetManager, new string?[,,] {
            {
                { "floor_1_righthalf", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" },
                { "floor_1_righthalf", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1_lefthalf" }
            },
            {
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, "fountain_1_bottom", null, null, null, null, null, "fountain_2_bottom", null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null },
                { null, null, null, null,                null, null, null, null, null, null,                null, null, null }
            },
            {
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, "fountain_1_top", null, null, null, null, null, "fountain_2_top", null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null },
                { null, null, null, null,             null, null, null, null, null, null,             null, null, null }
            },
            {
                { "wall_topleft_outer",    null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_topright_outer"    },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_bottomleft_outer", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottomright_outer" }
            }
        }, 1);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(true, false, _context.AssetManager.Get<PlayerType>("mage").Speed), [new PlayerBehaviourComponent(_context.AssetManager.Get<PlayerType>("mage"))]));
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>("slime").Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>("slime"))]));
        _world.Entities[1].PositionDataComponent.Position = new Vector2(5.0f, 0.0f);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>("slime").Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>("slime"))]));
        _world.Entities[2].PositionDataComponent.Position = new Vector2(15.0f, 4.0f);
        _world.Entities.Add(new Entity(_context.AssetManager, new LivingDataComponent(false, false, _context.AssetManager.Get<EnemyType>("slime").Speed), [new EnemyBehaviourComponent(_context.AssetManager.Get<EnemyType>("slime"))]));
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

    public void GetUniformData(string shaderProgram, string textureAtlas, Span<byte> buffer) {
        if (shaderProgram == "shader.userinterface") {
            UserInterfaceShaderLayout.UniformData data = new UserInterfaceShaderLayout.UniformData {
                Projection = _uiCamera.GetViewMatrix()
            };
            MemoryMarshal.Cast<UserInterfaceShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).CopyTo(buffer);
            return;
        } else if (shaderProgram == "shader.tilemap") {
            TextureAtlas<string> atlas = _context.AssetManager.Get<TextureAtlas<string>>(textureAtlas);
            TilemapShaderLayout.UniformData data = new TilemapShaderLayout.UniformData {
                Projection = _worldCamera.GetViewMatrix(),
                TimeMilliseconds = new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                Alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
                TextureSize = new Vector2i(atlas.AtlasWidth, atlas.AtlasHeight),
                CellSize = new Vector2i(atlas.CellWidth, atlas.CellHeight),
                Padding = atlas.CellPadding
            };
            MemoryMarshal.Cast<TilemapShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).CopyTo(buffer);
        }
    }

    public void Dispose() {
        _renderer.Dispose();
    }
}