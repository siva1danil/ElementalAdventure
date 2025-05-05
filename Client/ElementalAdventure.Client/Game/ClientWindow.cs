using System.Diagnostics;

using ElementalAdventure.Client.Core.Assets;
using ElementalAdventure.Client.Core.Resources.Composed;
using ElementalAdventure.Client.Core.Resources.OpenGL;
using ElementalAdventure.Client.Game.Data;
using ElementalAdventure.Client.Game.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ElementalAdventure.Client.Game;

public class ClientWindow : GameWindow {
    private readonly ClientContext _context;
    private IScene? _scene;

    /* temp */
    private double _gpuFrameTimeMs = 0.0;
    private double _gpuFrameTimeAccum = 0.0;
    private int _gpuFrameCount = 0;
    private double _gpuReportTimer = 0.0;
    private readonly Stopwatch _gpuTimer = new();
    /* temp */

    public ClientWindow(string root) : base(GameWindowSettings.Default, new NativeWindowSettings { ClientSize = new(1280, 720) }) {
        Load += LoadHandler;
        Unload += UnloadHandler;
        UpdateFrame += UpdateFrameHandler;
        RenderFrame += RenderFrameHandler;
        Resize += ResizeHandler;
        KeyDown += KeyDownHandler;
        KeyUp += KeyUpHandler;

        _context = new ClientContext(new AssetLoader(Path.Combine(root, "Resources")), new AssetManager<string>(), ClientSize);
    }

    private void LoadHandler() {
        try {
            _context.AssetManager.Add("shader.tilemap", new ShaderProgram(_context.AssetLoader.LoadText("Shader/Tilemap/Tilemap.vert"), _context.AssetLoader.LoadText("Shader/Tilemap/Tilemap.frag")));
            _context.AssetManager.Add("textureatlas.dungeon", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
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
            _context.AssetManager.Add("textureatlas.player", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "mage_idle_left", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_left.7.png")
                ], 100) },
                { "mage_idle_right", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_idle_right.7.png")
                ], 100) },
                { "mage_walk_left", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_left.7.png")
                ], 100) },
                { "mage_walk_right", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Player/mage_walk_right.7.png")
                ], 100) }
            }, 1));
            _context.AssetManager.Add("textureatlas.enemy", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "slime_walk_left", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_left.3.png")
                ], 100) },
                { "slime_walk_right", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Enemy/slime_walk_right.3.png")
                ], 100) }
            }, 1));

            _context.AssetManager.Add("font.pixeloidsans", new Font(_context.AssetLoader.LoadBinary("Font/Pixeloid/PixeloidSans.ttf"), [new Font.Range(' ', '~')], 16, 1));

            _context.AssetManager.Add("null", new TileType("textureatlas.dungeon", "null", 0, -0.5f));
            _context.AssetManager.Add("floor_1", new TileType("textureatlas.dungeon", "floor_1", 0, -0.5f));
            _context.AssetManager.Add("floor_1_bottomhalf", new TileType("textureatlas.dungeon", "floor_1_bottomhalf", 0, -0.5f));
            _context.AssetManager.Add("floor_1_lefthalf", new TileType("textureatlas.dungeon", "floor_1_lefthalf", 0, -0.5f));
            _context.AssetManager.Add("floor_1_righthalf", new TileType("textureatlas.dungeon", "floor_1_righthalf", 0, -0.5f));
            _context.AssetManager.Add("floor_1_tophalf", new TileType("textureatlas.dungeon", "floor_1_tophalf", 0, -0.5f));
            _context.AssetManager.Add("floor_2", new TileType("textureatlas.dungeon", "floor_2", 0, -0.5f));
            _context.AssetManager.Add("floor_2_bottomhalf", new TileType("textureatlas.dungeon", "floor_2_bottomhalf", 0, -0.5f));
            _context.AssetManager.Add("floor_2_lefthalf", new TileType("textureatlas.dungeon", "floor_2_lefthalf", 0, -0.5f));
            _context.AssetManager.Add("floor_2_righthalf", new TileType("textureatlas.dungeon", "floor_2_righthalf", 0, -0.5f));
            _context.AssetManager.Add("floor_2_tophalf", new TileType("textureatlas.dungeon", "floor_2_tophalf", 0, -0.5f));
            _context.AssetManager.Add("wall_bottom", new TileType("textureatlas.dungeon", "wall_bottom", 0, -0.5f));
            _context.AssetManager.Add("wall_left", new TileType("textureatlas.dungeon", "wall_left", 0, -0.5f));
            _context.AssetManager.Add("wall_right", new TileType("textureatlas.dungeon", "wall_right", 0, -0.5f));
            _context.AssetManager.Add("wall_top", new TileType("textureatlas.dungeon", "wall_top", 0, -0.5f));
            _context.AssetManager.Add("wall_bottomleft_inner", new TileType("textureatlas.dungeon", "wall_bottomleft_inner", 0, -0.5f));
            _context.AssetManager.Add("wall_bottomright_inner", new TileType("textureatlas.dungeon", "wall_bottomright_inner", 0, -0.5f));
            _context.AssetManager.Add("wall_topleft_inner", new TileType("textureatlas.dungeon", "wall_topleft_inner", 0, -0.5f));
            _context.AssetManager.Add("wall_topright_inner", new TileType("textureatlas.dungeon", "wall_topright_inner", 0, -0.5f));
            _context.AssetManager.Add("wall_bottomleft_outer", new TileType("textureatlas.dungeon", "wall_bottomleft_outer", 0, -0.5f));
            _context.AssetManager.Add("wall_bottomright_outer", new TileType("textureatlas.dungeon", "wall_bottomright_outer", 0, -0.5f));
            _context.AssetManager.Add("wall_topleft_outer", new TileType("textureatlas.dungeon", "wall_topleft_outer", 0, -0.5f));
            _context.AssetManager.Add("wall_topright_outer", new TileType("textureatlas.dungeon", "wall_topright_outer", 0, -0.5f));
            _context.AssetManager.Add("fountain_1_bottom", new TileType("textureatlas.dungeon", "fountain_1_bottom", 0, -0.5f + 7.0f / 32.0f));
            _context.AssetManager.Add("fountain_1_top", new TileType("textureatlas.dungeon", "fountain_1_top", -1, -0.5f - 1.0f + 7.0f / 32.0f));
            _context.AssetManager.Add("fountain_2_bottom", new TileType("textureatlas.dungeon", "fountain_2_bottom", 0, -0.5f + 7.0f / 32.0f));
            _context.AssetManager.Add("fountain_2_top", new TileType("textureatlas.dungeon", "fountain_2_top", -1, -0.5f - 1.0f + 7.0f / 32.0f));

            _context.AssetManager.Add("mage", new PlayerType("textureatlas.player", "mage_idle_left", "mage_idle_right", "mage_walk_left", "mage_walk_right", 0, -0.5f + 2.0f / 32.0f, 0.25f));
            _context.AssetManager.Add("slime", new EnemyType("textureatlas.enemy", "slime_walk_left", "slime_walk_right", "slime_walk_left", "slime_walk_right", 0, -0.5f + 2.0f / 32.0f, 0.125f));
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
        /* temp */
        _gpuTimer.Restart();

        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _scene?.Render(args);
        GL.Finish();

        _gpuTimer.Stop();
        _gpuFrameTimeAccum += _gpuTimer.Elapsed.TotalMilliseconds;
        _gpuFrameCount++;
        _gpuReportTimer += args.Time;
        if (_gpuReportTimer >= 1.0) {
            _gpuFrameTimeMs = _gpuFrameTimeAccum / _gpuFrameCount;
            Console.WriteLine($"[GPU] Avg Frame Time: {_gpuFrameTimeMs * 1000:F3} µs over {_gpuFrameCount} frames");
            _gpuFrameTimeAccum = 0.0;
            _gpuFrameCount = 0;
            _gpuReportTimer = 0.0;
        }

        SwapBuffers();
        /* temp */
    }

    private void ResizeHandler(ResizeEventArgs e) {
        GL.Viewport(0, 0, e.Width, e.Height);
        _context.WindowSize = new(e.Width, e.Height);
        _scene?.Resize(e);
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