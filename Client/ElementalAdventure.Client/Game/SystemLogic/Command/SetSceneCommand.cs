using ElementalAdventure.Client.Game.Scenes;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public class SetSceneCommand : IClientCommand {
    private readonly IScene _scene;

    public SetSceneCommand(IScene scene) {
        _scene = scene;
    }

    public void Execute(ClientWindow client, IScene? scene, ClientContext context) {
        client.SetScene(_scene);
    }
}