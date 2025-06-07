using ElementalAdventure.Client.Game.Components.Data;
using ElementalAdventure.Client.Game.Scenes;
using ElementalAdventure.Common.Assets;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game.SystemLogic.Command;

public class SpawnEntityCommand : IClientCommand {
    private readonly AssetID _entityType;
    private readonly Vector2 _position;

    public SpawnEntityCommand(AssetID entityType, Vector2 position) {
        _entityType = entityType;
        _position = position;
    }

    public void Execute(ClientWindow client, IScene? scene, ClientContext context) {
        if (scene is not GameScene gameScene) {
            context.CommandQueue.Enqueue(new CrashCommand("Expected active scene to be GameScene, got " + scene?.GetType().Name));
            return;
        }
        EnemyType enemyType = context.AssetManager.Get<EnemyType>(_entityType);
        gameScene.SpawnEnemy(enemyType, _position);
    }
}