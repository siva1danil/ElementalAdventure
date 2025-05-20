using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Common.Logging;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public class CrashCommand : IClientCommand {
    private readonly string _reason;

    public CrashCommand(string reason) {
        _reason = reason;
    }

    public void Execute(ClientWindow window, IScene? scene, ClientContext context) {
        Logger.Error(_reason);
        window.Close();
    }
}