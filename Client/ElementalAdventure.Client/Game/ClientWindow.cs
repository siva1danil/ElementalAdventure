using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Core.Resources;
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

        _context = new ClientContext(new ResourceLoader(Path.Combine(root, "Resources")), new ResourceRegistry(), ClientSize);
    }

    private void LoadHandler() {
        try {
            _context.ResourceRegistry.AddShader("shader.tilemap", new ShaderProgram(_context.ResourceLoader.LoadText("Shader/Tilemap.vert"), _context.ResourceLoader.LoadText("Shader/Tilemap.frag")));
            _context.ResourceRegistry.AddTextureAtlas("textureatlas.minecraft", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "chest_front", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/chest_front.png")], 100) },
                { "clay", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/clay.png")], 100) },
                { "coal_ore", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/coal_ore.png")], 100) },
                { "coarse_dirt", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/coarse_dirt.png")], 100) },
                { "cobblestone", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/cobblestone.png")], 100) },
                { "cobblestone_mossy", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/cobblestone_mossy.png")], 100) },
                { "diamond_ore", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/diamond_ore.png")], 100) },
                { "dirt", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/dirt.png")], 100) },
                { "stone", new([_context.ResourceLoader.LoadBinary("TextureAtlas/Minecraft/stone.png")], 100) }
            }, 1));
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            Close();
        }

        _scene = new GameScene(_context);
    }

    private void UnloadHandler() {
        _scene?.Dispose();
        _context.ResourceRegistry.Dispose();
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