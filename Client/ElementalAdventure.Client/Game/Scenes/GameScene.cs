using ElementalAdventure.Client.Core.Resources;

namespace ElementalAdventure.Client.Game.Scenes;

public class GameScene : IScene {
    private readonly ResourceRegistry _resourceRegistry;

    public GameScene(ResourceRegistry resourceRegistry) {
        _resourceRegistry = resourceRegistry;
    }

    public void Update() {
        //
    }

    public void Render() {
        //
    }

    public void Dispose() {
        //
    }
}