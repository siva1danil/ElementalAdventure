using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Client.Game.WorldLogic.Command;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public class SetTilemapCommand : IClientCommand {
    private readonly AssetID[,,] _tilemap;
    private readonly Box2[] _walls;
    private readonly int _midground;
    private readonly Vector2 _entrance;

    public SetTilemapCommand(AssetID[,,] tilemap, Box2[] walls, int midground, Vector2 entrance) {
        _tilemap = tilemap;
        _walls = walls;
        _midground = midground;
        _entrance = entrance;
    }

    public void Execute(ClientWindow client, IScene? scene, ClientContext context) {
        if (scene is not GameScene gameScene) {
            context.CommandQueue.Enqueue(new CrashCommand("Expected active scene to be GameScene, got " + scene?.GetType().Name));
            return;
        }
        gameScene.SetTilemap(_tilemap, _walls, _midground);
        gameScene.SetPlayerPosition(_entrance);
    }
}