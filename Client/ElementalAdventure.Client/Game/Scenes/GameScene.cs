using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic;
using ElementalAdventure.Client.Game.Logic.Command;
using ElementalAdventure.Client.Game.Logic.GameObject;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ClientContext _context;

    private readonly VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData> _tilemapVertexArray;
    private readonly VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData> _entityVertexArray;
    private readonly UniformBuffer<TilemapShaderLayout.UniformData> _uniformBuffer;
    private readonly GameWorld _world;

    private double _tickAccumulator;

    public GameScene(ClientContext context) {
        _context = context;

        _tilemapVertexArray = new VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData>();
        _entityVertexArray = new VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData>();
        _uniformBuffer = new UniformBuffer<TilemapShaderLayout.UniformData>();

        _world = new GameWorld(1.0f / 20.0f, new Tilemap(), []);
        _world.Tilemap.SetMap(_context.AssetManager, new string?[,,] {
            {
                { "null", "null", "null", "null", "null", "null", "null" },
                { "null", null,   null,   null,   null,   null,   "null" },
                { "null", null,   null,   null,   null,   null,   "null" },
                { "null", null,   null,   null,   null,   null,   "null" },
                { "null", null,   null,   null,   null,   null,   "null" },
                { "null", null,   null,   null,   null,   null,   "null" },
                { "null", "null", "null", "null", "null", "null", "null" }
            },
            {
                { "floor_1", "wall_top", "wall_top", "wall_top", "wall_top", "wall_top", "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" },
                { "floor_1", "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1",  "floor_1" }
            },
            {
                { "wall_topleft_outer",    null,          null,          null,          null,          null,          "wall_topright_outer"    },
                { "wall_left",             null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_left",             null,          null,          null,          null,          null,          "wall_right"             },
                { "wall_bottomleft_outer", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottom", "wall_bottomright_outer" }
            }
        });
        _world.Entities.Add(new Entity(_context.AssetManager, _context.AssetManager.GetEntityType("player"), new(2.0f, 0.0f), true));
        _world.Entities.Add(new Entity(_context.AssetManager, _context.AssetManager.GetEntityType("player"), new(0.0f, 2.0f), true));

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
        /* Init */
        ShaderProgram shader = _context.AssetManager.GetShader("shader.tilemap");
        TextureAtlas<string> atlasMinecraft = _context.AssetManager.GetTextureAtlas("textureatlas.dungeon");
        TextureAtlas<string> atlasPlayer = _context.AssetManager.GetTextureAtlas("textureatlas.player");

        long timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        TilemapShaderLayout.UniformData uniform = new() {
            Projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1),
            TimeMilliseconds = new Vector2i((int)(uint)(timeMilliseconds >> 32), (int)(uint)(timeMilliseconds & 0xFFFFFFFF)),
            Alpha = (timeMilliseconds - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f
        };

        GL.UseProgram(shader.Id);

        /* Tilemap */
        uniform.TextureSize = new Vector2i(atlasMinecraft.AtlasWidth, atlasMinecraft.AtlasHeight);
        uniform.TileSize = new Vector2i(atlasMinecraft.EntryWidth, atlasMinecraft.EntryHeight);
        uniform.Padding = atlasMinecraft.EntryPadding;
        _uniformBuffer.SetData(ref uniform);
        if (_world.Tilemap.Dirty) {
            _tilemapVertexArray.SetGlobalData(_world.Tilemap.GetGlobalData(), BufferUsageHint.DynamicDraw);
            _tilemapVertexArray.SetInstanceData(_world.Tilemap.GetInstanceData(), BufferUsageHint.DynamicDraw);
            _world.Tilemap.Dirty = false;
        }

        GL.BindVertexArray(_tilemapVertexArray.Id);
        GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffer.Id);
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffer.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlasMinecraft.Id);
        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _world.Tilemap.Count);

        /* Entities */
        uniform.TextureSize = new Vector2i(atlasPlayer.AtlasWidth, atlasPlayer.AtlasHeight);
        uniform.TileSize = new Vector2i(atlasPlayer.EntryWidth, atlasPlayer.EntryHeight);
        uniform.Padding = atlasPlayer.EntryPadding;
        _uniformBuffer.SetData(ref uniform);

        GL.BindVertexArray(_entityVertexArray.Id);
        GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffer.Id);
        GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffer.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlasPlayer.Id);
        foreach (Entity entity in _world.Entities) {
            _entityVertexArray.SetGlobalData(entity.GetGlobalData(), BufferUsageHint.DynamicDraw);
            _entityVertexArray.SetInstanceData(entity.GetInstanceData(), BufferUsageHint.DynamicDraw);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 1);
        }

        /* Cleanup */
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void KeyDown(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void KeyUp(KeyboardKeyEventArgs args) {
        _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));
    }

    public void Dispose() {
        //
    }
}