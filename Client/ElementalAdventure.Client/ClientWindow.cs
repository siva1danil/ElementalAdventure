using ElementalAdventure.Client.Scenes;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace ElementalAdventure.Client;

public class ClientWindow : GameWindow {
    private IScene? _scene;

    public ClientWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default) {
        Load += LoadHandler;
        UpdateFrame += UpdateFrameHandler;
        RenderFrame += RenderFrameHandler;
    }

    private void LoadHandler() {
        _scene = new ExampleScene();
    }

    private void UpdateFrameHandler(FrameEventArgs args) {
        if (_scene != null) _scene.Update();
    }

    private void RenderFrameHandler(FrameEventArgs args) {
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        if (_scene != null) _scene.Render();
        SwapBuffers();
    }

    public static void Main() => new ClientWindow().Run();
}