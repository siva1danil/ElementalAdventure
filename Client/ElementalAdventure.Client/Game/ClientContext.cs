using ElementalAdventure.Client.Core.Assets;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game;

public class ClientContext {
    public AssetLoader AssetLoader;
    public AssetManager<string> AssetManager;

    public Vector2 WindowSize;
    public HashSet<Keys> PressedKeys;

    public ClientContext(AssetLoader assetLoader, AssetManager<string> assetManager, Vector2 windowSize) {
        AssetLoader = assetLoader;
        AssetManager = assetManager;

        WindowSize = windowSize;
        PressedKeys = [];
    }
}