using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using ElementalAdventure.Common;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Networking;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Server.Storage;

namespace ElementalAdventure.Server;

public class GameServer {
    private readonly IPEndPoint _endpoint;
    private readonly PacketRegistry _registry;
    private readonly IDatabase _database;
    private readonly PacketServer _server;

    public GameServer(IPEndPoint endpoint, IDatabase database) {
        _endpoint = endpoint;
        _database = database;
        _registry = new PacketRegistry();
        _server = new PacketServer(_registry, endpoint);

        _registry.RegisterPacket(PacketType.HandshakeRequest, HandshakeRequestPacket.Deserialize, (conn, packet) => Logger.Info("Handshake request received"));

        _server.OnClientConnected += conn => Logger.Info($"Client connected");
        _server.OnClientDisconnected += (conn, ex) => Logger.Info($"Client disconnected");
        _server.OnPacketReceived += (conn, packet) => Logger.Info($"Packet received: {packet.GetType().Name}");
    }

    public async Task Run() {
        Stopwatch stopwatch = Stopwatch.StartNew();

        Logger.Info($"Connecting to database ({_database.GetType().Name})...");
        _database.Connect();
        Logger.Info($"Starting PacketServer at {_endpoint.Address}:{_endpoint.Port}...");
        _server.Start();

        stopwatch.Stop();
        Logger.Info($"Done! Loading took {stopwatch.ElapsedMilliseconds} ms.");
        if (_server.Awaiter != null) await _server.Awaiter;
    }

    public static async Task Test() {
        CancellationTokenSource cts = new();

        PacketRegistry registry = new PacketRegistry();
        PacketClient client = new PacketClient(registry, new IPEndPoint(IPAddress.Loopback, 12345));
        client.OnConnected += () => {
            Logger.Info($"Client connected");
            HandshakeRequestPacket packet = new HandshakeRequestPacket();
            client.Connection?.Send(packet);
        };
        client.OnDisconnected += (ex) => Logger.Info($"Client disconnected");
        client.OnPacketReceived += (packet) => Logger.Info($"Packet received: {packet.GetType().Name}");
        client.Start(cts.Token);
        Logger.Info($"Client started");
        if (client.Awaiter != null)
            await client.Awaiter;
    }

    public static async Task Main() {
        try {
            Logger.Info($"Elemental Adventure server '{BuildInfo.AssemblyTitle}' version '{BuildInfo.AssemblyVersion}'");
            GameServer server = new GameServer(new IPEndPoint(IPAddress.Any, 12345), new SQLiteDatabase("Data Source=D:\\db.sqlite3;"));
            await server.Run();
        } catch (Exception ex) {
            Logger.Error($"An error occurred: {ex.Message}");
        } finally {
            Logger.Info("Server shutting down...");
        }
    }
}