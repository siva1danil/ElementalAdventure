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
        // TODO: Render tilemap
        // TODO: Render UI
    }

    public void Dispose() {
        //
    }
}