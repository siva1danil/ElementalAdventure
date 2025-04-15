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
    private readonly GameWorld _world;

    private double _tickAccumulator;

    public GameScene(ClientContext context) {
        _context = context;

        _tilemapVertexArray = new VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData>();
        _entityVertexArray = new VertexArrayInstanced<TilemapShaderLayout.GlobalData, TilemapShaderLayout.InstanceData>();

        _world = new GameWorld((int)(1.0f / 20.0f * 1000f), new Tilemap(), []);
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
        while (_tickAccumulator >= _world.TickInterval / 1000f) {
            _tickAccumulator -= _world.TickInterval / 1000f;

            _world.AddCommand(new SetMovementCommand(_context.PressedKeys.Contains(Keys.W), _context.PressedKeys.Contains(Keys.A), _context.PressedKeys.Contains(Keys.S), _context.PressedKeys.Contains(Keys.D)));

            _world.Tick();
        }
    }

    public void Render(FrameEventArgs args) {
        ShaderProgram shader = _context.AssetManager.GetShader("shader.tilemap");
        TextureAtlas<string> atlasMinecraft = _context.AssetManager.GetTextureAtlas("textureatlas.dungeon");
        TextureAtlas<string> atlasPlayer = _context.AssetManager.GetTextureAtlas("textureatlas.player");
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1);
        long timeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        float alpha = (timeMilliseconds - _world.TickTimestamp) / (float)_world.TickInterval;

        GL.UseProgram(shader.Id);

        /* Tilemap */
        GL.BindVertexArray(_tilemapVertexArray.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlasMinecraft.Id);
        if (_world.Tilemap.Dirty) {
            _tilemapVertexArray.SetGlobalData(_world.Tilemap.GetGlobalData(), BufferUsageHint.DynamicDraw);
            _tilemapVertexArray.SetInstanceData(_world.Tilemap.GetInstanceData(), BufferUsageHint.DynamicDraw);
            _world.Tilemap.Dirty = false;
        }
        GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, "uProjection"), false, ref projection);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTimeMilliseconds"), (uint)(timeMilliseconds >> 32), (uint)(timeMilliseconds & 0xFFFFFFFF));
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uAlpha"), alpha);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTextureSize"), atlasMinecraft.AtlasWidth, atlasMinecraft.AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTileSize"), atlasMinecraft.EntryWidth, atlasMinecraft.EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uPadding"), atlasMinecraft.EntryPadding);
        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _world.Tilemap.Count);

        /* Entities */
        GL.BindVertexArray(_entityVertexArray.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlasPlayer.Id);
        GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, "uProjection"), false, ref projection);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTimeMilliseconds"), (uint)(timeMilliseconds >> 32), (uint)(timeMilliseconds & 0xFFFFFFFF));
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uAlpha"), alpha);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTextureSize"), atlasPlayer.AtlasWidth, atlasPlayer.AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTileSize"), atlasPlayer.EntryWidth, atlasPlayer.EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uPadding"), atlasPlayer.EntryPadding);
        foreach (Entity entity in _world.Entities) {
            _entityVertexArray.SetGlobalData(entity.GetGlobalData(), BufferUsageHint.DynamicDraw);
            _entityVertexArray.SetInstanceData(entity.GetInstanceData(), BufferUsageHint.DynamicDraw);
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 1);
        }

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        //
    }
}