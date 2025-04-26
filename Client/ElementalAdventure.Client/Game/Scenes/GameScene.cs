using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Logic;
using ElementalAdventure.Client.Game.Logic.Command;
using ElementalAdventure.Client.Game.Logic.Component.Behaviour;
using ElementalAdventure.Client.Game.Logic.Component.Data;
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
        ShaderProgram shader = _context.AssetManager.Get<ShaderProgram>("shader.tilemap");
        TextureAtlas<string> atlasDungeon = _context.AssetManager.Get<TextureAtlas<string>>("textureatlas.dungeon");

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

            // Entity
            foreach (Entity entity in _world.Entities) {
                if (!entity.TextureDataComponent.Visible) continue;

                TextureAtlas<string> atlas = _context.AssetManager.Get<TextureAtlas<string>>(entity.TextureDataComponent.TextureAtlas!);

                // Entity: set uniforms
                uniform = new() {
                    Projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1),
                    TimeMilliseconds = new Vector2i((int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >> 32), (int)(uint)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xFFFFFFFF)),
                    Alpha = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _world.TickTimestamp) / (float)_world.TickInterval / 1000.0f,
                    TextureSize = new Vector2i(atlas.AtlasWidth, atlas.AtlasHeight),
                    TileSize = new Vector2i(atlas.EntryWidth, atlas.EntryHeight),
                    Padding = atlas.EntryPadding
                };
                _uniformBuffer.SetData(ref uniform);

                // Entity: set vertex array
                _entityVertexArray.SetGlobalData(entity.GetGlobalData(), BufferUsageHint.DynamicDraw);
                _entityVertexArray.SetInstanceData(entity.GetInstanceData(), BufferUsageHint.DynamicDraw);

                // Entity: draw
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, atlas.Id);
                {
                    GL.BindBuffer(BufferTarget.UniformBuffer, _uniformBuffer.Id);
                    GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, _uniformBuffer.Id);
                    {
                        GL.BindVertexArray(_entityVertexArray.Id);
                        {
                            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, 1);
                        }
                        GL.BindVertexArray(0);
                    }
                    GL.BindBuffer(BufferTarget.UniformBuffer, 0);
                }
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
        _tilemapVertexArray.Dispose();
        _entityVertexArray.Dispose();
        _uniformBuffer.Dispose();
    }
}