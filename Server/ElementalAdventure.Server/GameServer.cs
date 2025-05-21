using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using ElementalAdventure.Common;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Networking;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;
using ElementalAdventure.Server.Models;
using ElementalAdventure.Server.PacketHandlers;
using ElementalAdventure.Server.Storage;
using ElementalAdventure.Server.World;

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

        _registry.RegisterPacket(PacketType.HandshakeRequest, HandshakeRequestPacket.Deserialize, new HandshakeRequestPacketHandler());
        _registry.RegisterPacket(PacketType.LoginRequest, LoginRequestPacket.Deserialize, new LoginRequestPacketHandler(_database));

        _server.OnClientConnected += conn => Logger.Info($"Client connected");
        _server.OnClientDisconnected += (conn, ex) => Logger.Info($"Client disconnected");
        _server.OnPacketReceived += (conn, packet) => _registry.TryHandlePacket(conn, packet);
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

    public static async Task Main() {
        // try {
        //     Logger.Info($"Elemental Adventure server '{BuildInfo.AssemblyTitle}' version '{BuildInfo.AssemblyVersion}'");
        //     GameServer server = new GameServer(new IPEndPoint(IPAddress.Any, 12345), new SQLiteDatabase("Data Source=D:\\db.sqlite3;"));
        //     await server.Run();
        // } catch (Exception ex) {
        //     Logger.Error($"An error occurred: {ex.Message}");
        // } finally {
        //     Logger.Info("Server shutting down...");
        // }

        Generator.LayoutRoom[,] layout = Generator.GenerateLayout(new Random().Next(), 0, new Dictionary<RoomType, int> { { RoomType.Entrance, 1 }, { RoomType.Exit, 1 }, { RoomType.Normal, 8 } });

        for (int y = 0; y < layout.GetLength(0); y++) {
            Console.Write($"{y,3}: ");
            for (int x = 0; x < layout.GetLength(1); x++) {
                var room = layout[y, x];
                Console.Write(room == Generator.LayoutRoom.Empty ? "   " : (room.DoorUp ? " | " : "   "));
            }
            Console.WriteLine();

            Console.Write($"{y,3}: ");
            for (int x = 0; x < layout.GetLength(1); x++) {
                var room = layout[y, x];
                if (room == Generator.LayoutRoom.Empty) {
                    Console.Write("   ");
                } else {
                    char c = room.Type switch { RoomType.Entrance => 'E', RoomType.Exit => 'X', RoomType.Normal => 'N', _ => '?' };
                    char left = room.DoorLeft ? '-' : ' ';
                    char right = room.DoorRight ? '-' : ' ';
                    Console.Write($"{left}{c}{right}");
                }
            }
            Console.WriteLine();

            Console.Write($"{y,3}: ");
            for (int x = 0; x < layout.GetLength(1); x++) {
                var room = layout[y, x];
                Console.Write(room == Generator.LayoutRoom.Empty ? "   " : (room.DoorDown ? " | " : "   "));
            }
            Console.WriteLine();
        }

        IsometricWorldType type = new(7, 5, new AssetID("floor_1"));
        Generator.TileMask[,] tilemask = Generator.GenerateTilemask(layout, type);
        for (int y = 0; y < tilemask.GetLength(0); y++) {
            Console.Write($"{y,3}: ");
            for (int x = 0; x < tilemask.GetLength(1); x++) {
                if (tilemask[y, x] == Generator.TileMask.None) Console.Write(" ");
                else Console.Write((byte)tilemask[y, x]);
            }
            Console.WriteLine();
        }

        AssetID[,,] tilemap = Generator.GenerateTilemap(tilemask, type);
        for (int l = 0; l < tilemap.GetLength(0); l++) {
            Console.WriteLine($"Layer {l}");
            for (int y = 0; y < tilemap.GetLength(1); y++) {
                Console.Write($"{y,3}: ");
                for (int x = 0; x < tilemap.GetLength(2); x++) {
                    if (tilemap[l, y, x] == AssetID.None) Console.Write(" ");
                    else Console.Write(tilemap[l, y, x].ToString()[0]);
                }
                Console.WriteLine();
            }
        }
    }
}