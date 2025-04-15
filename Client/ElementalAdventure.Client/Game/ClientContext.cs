using ElementalAdventure.Client.Game.Assets;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game;

public class ClientContext {
    public AssetLoader AssetLoader;
    public AssetManager AssetManager;

    public Vector2 WindowSize;
    public HashSet<Keys> PressedKeys;

    public ClientContext(AssetLoader assetLoader, AssetManager assetManager, Vector2 windowSize) {
        AssetLoader = assetLoader;
        AssetManager = assetManager;

        WindowSize = windowSize;
        PressedKeys = [];
    }
}