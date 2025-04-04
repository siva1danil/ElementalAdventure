using OpenTK.Windowing.Desktop;

namespace ElementalAdventure.Client;

public class ClientWindow : GameWindow {
    public ClientWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default) { }

    public static void Main() => new ClientWindow().Run();
}