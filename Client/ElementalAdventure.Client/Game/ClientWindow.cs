using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Game.Assets;
using ElementalAdventure.Client.Game.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ElementalAdventure.Client.Game;

public class ClientWindow : GameWindow {
    private readonly ClientContext _context;
    private IScene? _scene;

    public ClientWindow(string root) : base(GameWindowSettings.Default, new NativeWindowSettings { ClientSize = new(1280, 720) }) {
        Load += LoadHandler;
        Unload += UnloadHandler;
        UpdateFrame += UpdateFrameHandler;
        RenderFrame += RenderFrameHandler;
        Resize += ResizeHandler;
        KeyDown += KeyDownHandler;
        KeyUp += KeyUpHandler;

        _context = new ClientContext(new AssetLoader(Path.Combine(root, "Resources")), new AssetManager(), ClientSize);
    }

    private void LoadHandler() {
        try {
            _context.AssetManager.AddShader("shader.tilemap", new ShaderProgram(_context.AssetLoader.LoadText("Shader/Tilemap.vert"), _context.AssetLoader.LoadText("Shader/Tilemap.frag")));
            _context.AssetManager.AddTextureAtlas("textureatlas.dungeon", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "null", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/null.png")], 100) },
                { "floor_1", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1.png")], 100) },
                { "floor_1_bottomhalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_bottomhalf.png")], 100) },
                { "floor_1_lefthalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_lefthalf.png")], 100) },
                { "floor_1_righthalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_righthalf.png")], 100) },
                { "floor_1_tophalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_1_tophalf.png")], 100) },
                { "floor_2", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2.png")], 100) },
                { "floor_2_bottomhalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_bottomhalf.png")], 100) },
                { "floor_2_lefthalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_lefthalf.png")], 100) },
                { "floor_2_righthalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_righthalf.png")], 100) },
                { "floor_2_tophalf", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/floor_2_tophalf.png")], 100) },
                { "wall_bottom", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottom.png")], 100) },
                { "wall_left", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_left.png")], 100) },
                { "wall_right", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_right.png")], 100) },
                { "wall_top", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_top.png")], 100) },
                { "wall_bottomleft_inner", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottomleft_inner.png")], 100) },
                { "wall_bottomright_inner", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottomright_inner.png")], 100) },
                { "wall_topleft_inner", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_topleft_inner.png")], 100) },
                { "wall_topright_inner", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_topright_inner.png")], 100) },
                { "wall_bottomleft_outer", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottomleft_outer.png")], 100) },
                { "wall_bottomright_outer", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_bottomright_outer.png")], 100) },
                { "wall_topleft_outer", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_topleft_outer.png")], 100) },
                { "wall_topright_outer", new([_context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/wall_topright_outer.png")], 100) },
                { "fountain_1_bottom", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_bottom.4.png")
                ], 100) },
                { "fountain_1_top", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_1_top.4.png")
                ], 100) },
                { "fountain_2_bottom", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_bottom.4.png")
                ], 100) },
                { "fountain_2_top", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Dungeon/fountain_2_top.4.png")
                ], 100) }
            }, 1));
            _context.AssetManager.AddTextureAtlas("textureatlas.player", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "mage_idle", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle.7.png")
                ], 150) },
                { "mage_idle_right", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.7.png")
                ], 150) },
                { "mage_walk", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk.7.png")
                ], 150) },
                { "mage_walk_right", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.7.png")
                ], 150) }
            }, 1));

            _context.AssetManager.AddTileType("null", new("textureatlas.dungeon", "null"));
            _context.AssetManager.AddTileType("floor_1", new("textureatlas.dungeon", "floor_1"));
            _context.AssetManager.AddTileType("floor_1_bottomhalf", new("textureatlas.dungeon", "floor_1_bottomhalf"));
            _context.AssetManager.AddTileType("floor_1_lefthalf", new("textureatlas.dungeon", "floor_1_lefthalf"));
            _context.AssetManager.AddTileType("floor_1_righthalf", new("textureatlas.dungeon", "floor_1_righthalf"));
            _context.AssetManager.AddTileType("floor_1_tophalf", new("textureatlas.dungeon", "floor_1_tophalf"));
            _context.AssetManager.AddTileType("floor_2", new("textureatlas.dungeon", "floor_2"));
            _context.AssetManager.AddTileType("floor_2_bottomhalf", new("textureatlas.dungeon", "floor_2_bottomhalf"));
            _context.AssetManager.AddTileType("floor_2_lefthalf", new("textureatlas.dungeon", "floor_2_lefthalf"));
            _context.AssetManager.AddTileType("floor_2_righthalf", new("textureatlas.dungeon", "floor_2_righthalf"));
            _context.AssetManager.AddTileType("floor_2_tophalf", new("textureatlas.dungeon", "floor_2_tophalf"));
            _context.AssetManager.AddTileType("wall_bottom", new("textureatlas.dungeon", "wall_bottom"));
            _context.AssetManager.AddTileType("wall_left", new("textureatlas.dungeon", "wall_left"));
            _context.AssetManager.AddTileType("wall_right", new("textureatlas.dungeon", "wall_right"));
            _context.AssetManager.AddTileType("wall_top", new("textureatlas.dungeon", "wall_top"));
            _context.AssetManager.AddTileType("wall_bottomleft_inner", new("textureatlas.dungeon", "wall_bottomleft_inner"));
            _context.AssetManager.AddTileType("wall_bottomright_inner", new("textureatlas.dungeon", "wall_bottomright_inner"));
            _context.AssetManager.AddTileType("wall_topleft_inner", new("textureatlas.dungeon", "wall_topleft_inner"));
            _context.AssetManager.AddTileType("wall_topright_inner", new("textureatlas.dungeon", "wall_topright_inner"));
            _context.AssetManager.AddTileType("wall_bottomleft_outer", new("textureatlas.dungeon", "wall_bottomleft_outer"));
            _context.AssetManager.AddTileType("wall_bottomright_outer", new("textureatlas.dungeon", "wall_bottomright_outer"));
            _context.AssetManager.AddTileType("wall_topleft_outer", new("textureatlas.dungeon", "wall_topleft_outer"));
            _context.AssetManager.AddTileType("wall_topright_outer", new("textureatlas.dungeon", "wall_topright_outer"));
            _context.AssetManager.AddTileType("fountain_1_bottom", new("textureatlas.dungeon", "fountain_1_bottom"));
            _context.AssetManager.AddTileType("fountain_1_top", new("textureatlas.dungeon", "fountain_1_top"));
            _context.AssetManager.AddTileType("fountain_2_bottom", new("textureatlas.dungeon", "fountain_2_bottom"));
            _context.AssetManager.AddTileType("fountain_2_top", new("textureatlas.dungeon", "fountain_2_top"));

            _context.AssetManager.AddEntityType("player", new EntityType("textureatlas.player", "mage_idle", "mage_idle_right", "mage_walk_left", "mage_walk_right", 0.25f));
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            Close();
        }

        _scene = new GameScene(_context);

        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void UnloadHandler() {
        _scene?.Dispose();
        _context.AssetManager.Dispose();
    }

    private void UpdateFrameHandler(FrameEventArgs args) {
        _scene?.Update(args);
    }

    private void RenderFrameHandler(FrameEventArgs args) {
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _scene?.Render(args);
        SwapBuffers();
    }

    private void ResizeHandler(ResizeEventArgs e) {
        GL.Viewport(0, 0, e.Width, e.Height);
        _context.WindowSize = new(e.Width, e.Height);
    }

    private void KeyDownHandler(KeyboardKeyEventArgs e) {
        if (!_context.PressedKeys.Contains(e.Key)) {
            _context.PressedKeys.Add(e.Key);
            _scene?.KeyDown(e);
        }
    }

    private void KeyUpHandler(KeyboardKeyEventArgs e) {
        if (_context.PressedKeys.Contains(e.Key)) {
            _context.PressedKeys.Remove(e.Key);
            _scene?.KeyUp(e);
        }
    }

    public static void Main() => new ClientWindow(AppDomain.CurrentDomain.BaseDirectory).Run();
}