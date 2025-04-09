using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resources;
using ElementalAdventure.Client.Game.Model;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ClientContext _context;

    private readonly VertexArray<Tilemap.VertexData> _vertexArray;
    private readonly StorageBuffer<Tilemap.InstanceData> _instanceBuffer;
    private readonly Tilemap _tilemap;

    public GameScene(ClientContext context) {
        _context = context;

        _vertexArray = new VertexArray<Tilemap.VertexData>();
        _instanceBuffer = new StorageBuffer<Tilemap.InstanceData>();
        _tilemap = new Tilemap();
        _tilemap.SetMap(_context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft"), new string?[,] {
            {"cobblestone_mossy", "cobblestone_mossy", "cobblestone_mossy", "cobblestone_mossy"},
            {"cobblestone_mossy", "cobblestone_mossy", "cobblestone_mossy", "cobblestone_mossy"},
            {null, "coal_ore", "coal_ore", null},
            {"dirt", null, null, "dirt"},
        });
    }

    public void Update() {
        // 
    }

    public void Render() {
        GL.UseProgram(_context.ResourceRegistry.GetShader("shader.tilemap").Id);
        GL.BindVertexArray(_vertexArray.Id);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _instanceBuffer.Id);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _instanceBuffer.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").Id);

        if (_tilemap.Dirty) {
            _vertexArray.SetData(_tilemap.GetVertexData(), BufferUsageHint.StreamDraw);
            _instanceBuffer.SetData(_tilemap.GetInstanceData(), BufferUsageHint.StreamDraw);
        }

        Matrix4 matrix4 = Matrix4.CreateOrthographicOffCenter(-_context.WindowSize.X / 100, _context.WindowSize.X / 100, -_context.WindowSize.Y / 100, _context.WindowSize.Y / 100, -1, 1);
        GL.Uniform2(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uTextureSize"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasWidth, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uTileSize"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryWidth, _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uPadding"), _context.ResourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryPadding);
        GL.UniformMatrix4(GL.GetUniformLocation(_context.ResourceRegistry.GetShader("shader.tilemap").Id, "uProjection"), false, ref matrix4);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _tilemap.TileCount);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        //
    }
}