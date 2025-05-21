using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public class SetTilemapCommand : IClientCommand {
    private readonly AssetID[,,] _tilemap;
    private readonly int _midground;

    public SetTilemapCommand(AssetID[,,] tilemap, int midground) {
        _tilemap = tilemap;
        _midground = midground;
    }

    public void Execute(ClientWindow client, IScene? scene, ClientContext context) {
        if (scene is not GameScene gameScene) {
            context.CommandQueue.Enqueue(new CrashCommand("Expected active scene to be GameScene, got " + scene?.GetType().Name));
            return;
        }
        gameScene.SetTilemap(_tilemap, _midground);
    }
}