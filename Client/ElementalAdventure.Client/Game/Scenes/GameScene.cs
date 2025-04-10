using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Game.Model;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ClientContext _context;

    private readonly VertexArrayInstanced<Tilemap.GlobalData, Tilemap.InstanceData> _vertexArray;
    private readonly Tilemap _tilemap;

    public GameScene(ClientContext context) {
        _context = context;

        _vertexArray = new VertexArrayInstanced<Tilemap.GlobalData, Tilemap.InstanceData>();
        _tilemap = new Tilemap();
        _tilemap.SetMap(_context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft"), new string?[,,] {
            {
                {null, null, null, null},
                {null, null, null, null},
                {null, null, null, null},
                {null, null, null, null},
                {null, "dirt", "dirt", null},
                {null, null, null, null}
            },
            {
                {"chest_front", "chest_front", "chest_front", "chest_front"},
                {"chest_front", null, null, "chest_front"},
                {"chest_front", null, null, "chest_front"},
                {"chest_front", null, null, "chest_front"},
                {"chest_front", "fire", "fire", "chest_front"},
                {"chest_front", "chest_front", "chest_front", "chest_front"}
            }
        });
    }

    public void Update() {
        // 
    }

    public void Render() {
        GL.UseProgram(_context.ResourceRegistry.GetShader("shader.tilemap").Id);
        GL.BindVertexArray(_vertexArray.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").Id);

        _vertexArray.SetGlobalData(_tilemap.GetGlobalData(), BufferUsageHint.StreamDraw);
        _vertexArray.SetInstanceData(_tilemap.GetInstanceData(), BufferUsageHint.StreamDraw);

        Matrix4 matrix4 = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 120, 0, _context.WindowSize.Y / 120, -1, 1);
        GL.UniformMatrix4(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uProjection"), false, ref matrix4);
        GL.Uniform2(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uTimeMilliseconds"), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds >> 32), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds & 0xFFFFFFFF));
        GL.Uniform2(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uTextureSize"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasWidth, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uTileSize"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryWidth, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uPadding"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryPadding);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _tilemap.Count);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        //
    }
}