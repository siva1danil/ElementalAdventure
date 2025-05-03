using System.Runtime.InteropServices;

using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic;
using ElementalAdventure.Client.Game.Logic.Command;
using ElementalAdventure.Client.Game.Logic.Component.Behaviour;
using ElementalAdventure.Client.Game.Logic.Component.Data;
using ElementalAdventure.Client.Game.Logic.GameObject;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ClientContext _context;

    private readonly GameWorld _world;
    private readonly Camera _camera;

    private double _tickAccumulator;

    public GameScene(ClientContext context) {
        _context = context;

        _world = new GameWorld(1.0f / 20.0f, new Tilemap(), []);
        _camera = new Camera(new Vector2(13.0f * 0.5f - 0.5f, 9.0f * 0.5f - 0.5f), new Vector2(14.0f, 10f), context.WindowSize);
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
        // TODO: use new renderer
    }

    public void Resize(ResizeEventArgs args) {
        _camera.ScreenSize = new Vector2(args.Size.X, args.Size.Y);
    }

    public void KeyDown(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void KeyUp(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public byte[] GetUniformData(string shaderProgram, string textureAtlas) {
        if (shaderProgram != "shader.tilemap")
            throw new ArgumentException($"Unexpected shader program: {shaderProgram}");
        TextureAtlas<string> atlas = _context.AssetManager.Get<TextureAtlas<string>>(textureAtlas);
        TilemapShaderLayout.UniformData data = new TilemapShaderLayout.UniformData {
            Projection = _camera.GetViewMatrix(),
            TimeMilliseconds = new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
            Alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
            TextureSize = new Vector2i(atlas.AtlasWidth, atlas.AtlasHeight),
            CellSize = new Vector2i(atlas.CellWidth, atlas.CellHeight),
            Padding = atlas.CellPadding
        };
        return MemoryMarshal.Cast<TilemapShaderLayout.UniformData, byte>(MemoryMarshal.CreateSpan(ref data, 1)).ToArray();
    }

    public void Dispose() {
        //
    }
}