using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Logic;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ClientContext _context;

    private readonly VertexArrayInstanced<Tilemap.GlobalData, Tilemap.InstanceData> _tilemapVertexArray;
    private readonly VertexArrayInstanced<Entity.GlobalData, Entity.InstanceData> _entityVertexArray;
    private readonly GameWorld _world;

    public GameScene(ClientContext context) {
        _context = context;

        _tilemapVertexArray = new VertexArrayInstanced<Tilemap.GlobalData, Tilemap.InstanceData>();
        _entityVertexArray = new VertexArrayInstanced<Entity.GlobalData, Entity.InstanceData>();

        _world = new GameWorld(new Tilemap(), []);
        _world.Tilemap.SetMap(_context.AssetManager, new string?[,,] {
            {
                { null, null, null, null },
                {"grass", "grass", "grass", "grass"},
                {"dirt", "dirt", "dirt", "dirt"},
                {"stone", "stone", "dirt", "dirt"},
                {"stone", "stone", "stone", "stone"},
                {"stone", "stone", "stone", "stone"},
                {"stone", "stone", "stone", "stone"},
                {"stone", "stone", "stone", "stone"},
            },
            {
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { "lava", "lava", "lava", "lava" },
            },
            {
                { "fire", null, null, null },
                { null, null, "water", "water" },
                { null, null, null, "water" },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
                { null, null, null, null },
            }
        });
        _world.Entities.Add(new Entity(_context.AssetManager, _context.AssetManager.GetEntityType("player"), new Vector3(0.0f, 0.0f, 1.0f)));
        _world.Entities.Add(new Entity(_context.AssetManager, _context.AssetManager.GetEntityType("player"), new Vector3(4.0f, 2.0f, 1.0f)));
    }

    public void Update(FrameEventArgs args) {
        // 
    }

    public void Render(FrameEventArgs args) {
        ShaderProgram shader = _context.AssetManager.GetShader("shader.tilemap");
        TextureAtlas<string> atlasMinecraft = _context.AssetManager.GetTextureAtlas("textureatlas.minecraft");
        TextureAtlas<string> atlasPlayer = _context.AssetManager.GetTextureAtlas("textureatlas.player");
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 80, 0, _context.WindowSize.Y / 80, -1, 1);

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
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTimeMilliseconds"), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds >> 32), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds & 0xFFFFFFFF));
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTextureSize"), atlasMinecraft.AtlasWidth, atlasMinecraft.AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTileSize"), atlasMinecraft.EntryWidth, atlasMinecraft.EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uPadding"), atlasMinecraft.EntryPadding);
        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _world.Tilemap.Count);

        /* Entities */
        GL.BindVertexArray(_entityVertexArray.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlasPlayer.Id);
        GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, "uProjection"), false, ref projection);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTimeMilliseconds"), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds >> 32), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds & 0xFFFFFFFF));
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