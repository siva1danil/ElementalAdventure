using ElementalAdventure.Client.Game.SystemLogic;

namespace ElementalAdventure.Client.Game;

public class GameClient {
    public static void Main() {
        ClientWindow window = new(AppDomain.CurrentDomain.BaseDirectory);
        window.Run();
    }
}