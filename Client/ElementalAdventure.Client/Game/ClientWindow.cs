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

        _context = new ClientContext(new AssetLoader(Path.Combine(root, "Resources")), new AssetManager(), ClientSize);
    }

    private void LoadHandler() {
        try {
            _context.AssetManager.AddShader("shader.tilemap", new ShaderProgram(_context.AssetLoader.LoadText("Shader/Tilemap.vert"), _context.AssetLoader.LoadText("Shader/Tilemap.frag")));
            _context.AssetManager.AddTextureAtlas("textureatlas.minecraft", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "grass", new([_context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/grass.png")], 100) },
                { "dirt", new([_context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/dirt.png")], 100) },
                { "stone", new([_context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/stone.png")], 100) },
                { "water", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_7.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_8.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_9.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_10.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_11.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_12.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_13.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_14.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_15.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_16.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_17.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_18.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_19.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_20.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_21.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_22.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_23.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_24.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_25.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_26.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_27.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_28.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_29.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_30.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/water_31.png")
                ], 50) },
                { "lava", new([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_7.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_8.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_9.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_10.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_11.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_12.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_13.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_14.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_15.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_16.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_17.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_18.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_19.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_20.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_21.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_22.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_23.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_24.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_25.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_26.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_27.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_28.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_29.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_30.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_31.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_32.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_33.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_34.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_35.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_36.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_37.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_38.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/lava_39.png")
                ], 50) },
                { "fire", new ([
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_0.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_1.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_2.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_3.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_4.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_5.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_6.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_7.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_8.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_9.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_10.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_11.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_12.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_13.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_14.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_15.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_16.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_17.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_18.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_19.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_20.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_21.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_22.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_23.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_24.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_25.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_26.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_27.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_28.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_29.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_30.png"),
                    _context.AssetLoader.LoadBinary("TextureAtlas/Minecraft/fire_31.png"),
                ], 50) },
            }, 1));
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            Close();
        }

        _scene = new GameScene(_context);

        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    private void UnloadHandler() {
        _scene?.Dispose();
        _context.AssetManager.Dispose();
    }

    private void UpdateFrameHandler(FrameEventArgs args) {
        _scene?.Update();
    }

    private void RenderFrameHandler(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _scene?.Render();
        SwapBuffers();
    }

    private void ResizeHandler(ResizeEventArgs e) {
        GL.Viewport(0, 0, e.Width, e.Height);
        _context.WindowSize = new(e.Width, e.Height);
    }

    public static void Main() => new ClientWindow(AppDomain.CurrentDomain.BaseDirectory).Run();
}