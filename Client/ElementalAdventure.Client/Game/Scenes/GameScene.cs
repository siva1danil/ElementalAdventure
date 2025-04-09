using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resources;
using ElementalAdventure.Client.Game.Model;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ResourceRegistry _resourceRegistry;

    private readonly VertexArray<Tilemap.VertexData> _vertexArray;
    private readonly StorageBuffer<Tilemap.InstanceData> _instanceBuffer;
    private readonly Tilemap _tilemap;

    public GameScene(ResourceRegistry resourceRegistry) {
        _resourceRegistry = resourceRegistry;

        _vertexArray = new VertexArray<Tilemap.VertexData>();
        _instanceBuffer = new StorageBuffer<Tilemap.InstanceData>();
        _tilemap = new Tilemap();
        _tilemap.SetMap(_resourceRegistry.GetTextureAtlas("textureatlas.minecraft"), new string?[,] {
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
        GL.UseProgram(_resourceRegistry.GetShader("shader.tilemap").Id);
        GL.BindVertexArray(_vertexArray.Id);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, _instanceBuffer.Id);
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 0, _instanceBuffer.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _resourceRegistry.GetTextureAtlas("textureatlas.minecraft").Id);

        if (_tilemap.Dirty) {
            _vertexArray.SetData(_tilemap.GetVertexData(), BufferUsageHint.StreamDraw);
            _instanceBuffer.SetData(_tilemap.GetInstanceData(), BufferUsageHint.StreamDraw);
        }

        GL.Uniform2(GL.GetUniformLocation(_resourceRegistry.GetShader("shader.tilemap").Id, "uTextureSize"), _resourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasWidth, _resourceRegistry.GetTextureAtlas("textureatlas.minecraft").AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(_resourceRegistry.GetShader("shader.tilemap").Id, "uTileSize"), _resourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryWidth, _resourceRegistry.GetTextureAtlas("textureatlas.minecraft").EntryHeight);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _tilemap.TileCount);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        //
    }
}