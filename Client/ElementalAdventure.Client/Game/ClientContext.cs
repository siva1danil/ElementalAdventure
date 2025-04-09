using ElementalAdventure.Client.Core.Resources;

using OpenTK.Mathematics;

namespace ElementalAdventure.Client.Game;

public class ClientContext {
    public ResourceLoader ResourceLoader;
    public ResourceRegistry ResourceRegistry;

    public Vector2 WindowSize;

    public ClientContext(ResourceLoader resourceLoader, ResourceRegistry resourceRegistry, Vector2 windowSize) {
        ResourceLoader = resourceLoader;
        ResourceRegistry = resourceRegistry;
        WindowSize = windowSize;
    }
}