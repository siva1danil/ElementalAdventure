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
        }, 2);
        _world.Entities.Add(new Entity(_context.AssetManager, _context.AssetManager.GetEntityType("player"), new(2.0f, 0.0f), false));
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
        ShaderProgram shader = _context.AssetManager.GetShader("shader.tilemap");
        TextureAtlas<string> atlasDungeon = _context.AssetManager.GetTextureAtlas("textureatlas.dungeon");
        TextureAtlas<string> atlasPlayer = _context.AssetManager.GetTextureAtlas("textureatlas.player");

        GL.UseProgram(shader.Id);
        {
            // Tilemap: set uniforms
            TilemapShaderLayout.UniformData uniform = new() {
                Projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1),
                TimeMilliseconds = new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                Alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
                TextureSize = new Vector2i(atlasDungeon.AtlasWidth, atlasDungeon.AtlasHeight),
                TileSize = new Vector2i(atlasDungeon.EntryWidth, atlasDungeon.EntryHeight),
                Padding = atlasDungeon.EntryPadding
            };
            _uniformBuffer.SetData(ref uniform);

            // Tilemap: set vertex array
            if (_world.Tilemap.Dirty) {
                _tilemapVertexArray.SetGlobalData(_world.Tilemap.GetGlobalData(), BufferUsageHint.DynamicDraw);
                _tilemapVertexArray.SetInstanceData(_world.Tilemap.GetInstanceData(), BufferUsageHint.DynamicDraw);
                _world.Tilemap.Dirty = false;
            }

            // Tilemap: draw
            GL.BindVertexArray(_tilemapVertexArray.Id);
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffer.Id);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffer.Id);
                {
                    GL.ActiveTexture(TextureUnit.Texture0);
                    GL.BindTexture(TextureTarget.Texture2D, atlasDungeon.Id);
                    {
                        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _world.Tilemap.Count);
                    }
                    GL.BindTexture(TextureTarget.Texture2D, 0);
                    GL.ActiveTexture(TextureUnit.Texture0);
                }
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
            GL.BindVertexArray(0);

            // Entity: set uniforms
            uniform = new() {
                Projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1),
                TimeMilliseconds = new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                Alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
                TextureSize = new Vector2i(atlasPlayer.AtlasWidth, atlasPlayer.AtlasHeight),
                TileSize = new Vector2i(atlasPlayer.EntryWidth, atlasPlayer.EntryHeight),
                Padding = atlasPlayer.EntryPadding
            };
            _uniformBuffer.SetData(ref uniform);

            // Entity: set vertex array and draw
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, atlasPlayer.Id);
            {
                GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffer.Id);
                GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffer.Id);
                {
                    foreach (Entity entity in _world.Entities) {
                        // Entity: set vertex array
                        _entityVertexArray.SetGlobalData(entity.GetGlobalData(), BufferUsageHint.DynamicDraw);
                        _entityVertexArray.SetInstanceData(entity.GetInstanceData(), BufferUsageHint.DynamicDraw);

                        // Entity: draw
                        GL.BindVertexArray(_entityVertexArray.Id);
                        {
                            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 1);
                        }
                        GL.BindVertexArray(0);
                    }
                }
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.ActiveTexture(TextureUnit.Texture0);
        }
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