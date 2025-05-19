using ElementalAdventure.Client.Game.SystemLogic;

namespace ElementalAdventure.Client.Game;

public class GameClient {
    public static void Main() {
        ClientWindow window = new(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 12345), AppDomain.CurrentDomain.BaseDirectory);
        window.Run();
    }
}