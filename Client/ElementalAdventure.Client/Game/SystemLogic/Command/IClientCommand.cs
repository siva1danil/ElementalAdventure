using ElementalAdventure.Client.Game.Scenes;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public interface IClientCommand {
    void Execute(ClientWindow client, IScene? scene, ClientContext context);
}