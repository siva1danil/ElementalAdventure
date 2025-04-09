using ElementalAdventure.Client.Core.OpenGL;
using ElementalAdventure.Client.Core.Resource;
using ElementalAdventure.Client.Core.Resources;
using ElementalAdventure.Client.Game.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ElementalAdventure.Client.Game;

public class ClientWindow : GameWindow {
    private readonly ResourceLoader _resourceLoader;
    private readonly ResourceRegistry _resourceRegistry;
    private IScene? _scene;

    public ClientWindow(string root) : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
        Load += LoadHandler;
        Unload += UnloadHandler;
        UpdateFrame += UpdateFrameHandler;
        RenderFrame += RenderFrameHandler;
        Resize += ResizeHandler;

        _resourceLoader = new ResourceLoader(Path.Combine(root, "Resources"));
        _resourceRegistry = new ResourceRegistry();
    }

    private void LoadHandler() {
        try {
            _resourceRegistry.AddShader("shader.tilemap", new ShaderProgram(_resourceLoader.LoadText("Shader/Tilemap.vert"), _resourceLoader.LoadText("Shader/Tilemap.frag")));
            _resourceRegistry.AddTextureAtlas("textureatlas.minecraft", new TextureAtlas<string>(new Dictionary<string, TextureAtlas<string>.EntryDef> {
                { "chest_front", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/chest_front.png")], 100) },
                { "clay", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/clay.png")], 100) },
                { "coal_ore", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/coal_ore.png")], 100) },
                { "coarse_dirt", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/coarse_dirt.png")], 100) },
                { "cobblestone", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/cobblestone.png")], 100) },
                { "cobblestone_mossy", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/cobblestone_mossy.png")], 100) },
                { "diamond_ore", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/diamond_ore.png")], 100) },
                { "dirt", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/dirt.png")], 100) },
                { "stone", new([_resourceLoader.LoadBinary("TextureAtlas/Minecraft/stone.png")], 100) }
            }));
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            Close();
        }

        _scene = new GameScene(_resourceRegistry);
    }

    private void UnloadHandler() {
        _scene?.Dispose();
        _resourceRegistry.Dispose();
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
    }

    public static void Main() => new ClientWindow(AppDomain.CurrentDomain.BaseDirectory).Run();
}