using ElementalAdventure.Client.Core.OpenGL;
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
            _resourceRegistry.AddShader("default", new ShaderProgram(_resourceLoader.LoadText("Shaders/Default.vert"), _resourceLoader.LoadText("Shaders/Default.frag")));
            _resourceRegistry.AddTexture("default", new Texture2D(_resourceLoader.LoadBinary("Textures/Default.png")));
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