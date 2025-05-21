using ElementalAdventure.Common;
using ElementalAdventure.Common.Assets;
using ElementalAdventure.Common.Logging;
using ElementalAdventure.Common.Packets;
using ElementalAdventure.Common.Packets.Impl;
using ElementalAdventure.Server.Storage;
using ElementalAdventure.Server.World;

namespace ElementalAdventure.Server.PacketHandlers;

public class LoadWorldRequestPacketHandler : PacketRegistry.IPacketHandler {
    private readonly IDatabase _database;

    public LoadWorldRequestPacketHandler(IDatabase database) {
        _database = database;
    }

    public void Handle(PacketConnection connection, IPacket ipacket) {
        if (ipacket is not LoadWorldRequestPacket packet) {
            Logger.Info("Expected LoadWorldRequestPacket, got " + ipacket.GetType().Name);
            return;
        }

        if (connection == null) {
            Logger.Error("Connection is null");
            return;
        }

        IsometricWorldType type = new(7, 5, new AssetID("floor_1"));
        Generator.LayoutRoom[,] layout = Generator.GenerateLayout(new Random().Next(), 0, new Dictionary<RoomType, int> { { RoomType.Entrance, 1 }, { RoomType.Exit, 1 }, { RoomType.Normal, 8 } });
        Generator.TileMask[,] tilemask = Generator.GenerateTilemask(layout, type);
        AssetID[,,] tilemap = Generator.GenerateTilemap(tilemask, type);
        int[,,] data = new int[tilemap.GetLength(0), tilemap.GetLength(1), tilemap.GetLength(2)];
        for (int z = 0; z < tilemap.GetLength(0); z++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
                for (int x = 0; x < tilemap.GetLength(2); x++)
                    data[z, y, x] = tilemap[z, y, x].Value;
        _ = connection.SendAsync(new LoadWorldResponsePacket() { Tilemap = data, Midground = type.MidgroundLayer });
    }
}