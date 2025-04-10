using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
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
        _tilemap.SetMap(_context.AssetManager.GetTextureAtlas("textureatlas.minecraft"), new string?[,,] {
            {
                {null, null, null, null},
                {null, null, null, null},
                {null, "water", "lava", null},
                {null, null, null, null},
                {null, "stone", "stone", null},
                {null, null, null, null}
            },
            {
                {"grass", "grass", "grass", "grass"},
                {"grass", null, null, "grass"},
                {"grass", null, null, "grass"},
                {"grass", null, null, "grass"},
                {"grass", "fire", "fire", "grass"},
                {"grass", "grass", "grass", "grass"}
            }
        });
    }

    public void Update() {
        // 
    }

    public void Render() {
        ShaderProgram shader = _context.AssetManager.GetShader("shader.tilemap");
        TextureAtlas<string> atlas = _context.AssetManager.GetTextureAtlas("textureatlas.minecraft");

        GL.UseProgram(shader.Id);
        GL.BindVertexArray(_vertexArray.Id);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, atlas.Id);

        if (_tilemap.Dirty) {
            _vertexArray.SetGlobalData(_tilemap.GetGlobalData(), BufferUsageHint.DynamicDraw);
            _vertexArray.SetInstanceData(_tilemap.GetInstanceData(), BufferUsageHint.DynamicDraw);
            _tilemap.Dirty = false;
        }

        Matrix4 matrix4 = Matrix4.CreateOrthographicOffCenter(0, _context.WindowSize.X / 120, 0, _context.WindowSize.Y / 120, -1, 1);
        GL.UniformMatrix4(GL.GetUniformLocation(shader.Id, "uProjection"), false, ref matrix4);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTimeMilliseconds"), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds >> 32), (uint)((long)DateTime.UtcNow.TimeOfDay.TotalMilliseconds & 0xFFFFFFFF));
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTextureSize"), atlas.AtlasWidth, atlas.AtlasHeight);
        GL.Uniform2(GL.GetUniformLocation(shader.Id, "uTileSize"), atlas.EntryWidth, atlas.EntryHeight);
        GL.Uniform1(GL.GetUniformLocation(shader.Id, "uPadding"), atlas.EntryPadding);

        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, 6, _tilemap.Count);

        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }

    public void Dispose() {
        //
    }
}