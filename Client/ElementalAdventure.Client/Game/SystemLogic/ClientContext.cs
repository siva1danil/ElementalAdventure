using ElementalAdventure.Client.Game.SystemLogic.Command;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Networking;
using ElementalAdventure.Common.Packets;

using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ElementalAdventure.Client.Game.SystemLogic;

public class ClientContext {
    public AssetLoader AssetLoader;
    public AssetManager AssetManager;
    public PacketRegistry PacketRegistry;
    public PacketClient PacketClient;
    public Queue<IClientCommand> CommandQueue;

    public Vector2 WindowSize;
    public HashSet<Keys> PressedKeys;

    public ClientContext(AssetLoader assetLoader, AssetManager assetManager, PacketRegistry packetRegistry, PacketClient client, Vector2 windowSize) {
        AssetLoader = assetLoader;
        AssetManager = assetManager;
        PacketRegistry = packetRegistry;
        PacketClient = client;
        CommandQueue = [];

        WindowSize = windowSize;
        PressedKeys = [];
    }
}