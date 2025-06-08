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

        IsometricWorldType type = new(11, 7, new AssetID("floor_1_full"), new AssetID("wall_bottom"), new AssetID("wall_top"), new AssetID("wall_top_connected"), new AssetID("wall_bottom_connected"), new AssetID("wall_left"), new AssetID("wall_right"), new AssetID("wall_righttop"), new AssetID("wall_rightbottom"), new AssetID("wall_lefttop"), new AssetID("wall_leftbottom"), new AssetID("wall_side_connected"), new AssetID("wall_cross_lb_rb"), new AssetID("wall_cross_lt_lb"), new AssetID("wall_cross_lt_rt"), new AssetID("wall_cross_rt_rb"), new AssetID("wall_cross_lt_lb_rb"), new AssetID("wall_cross_lt_rt_rb"), new AssetID("wall_cross_lb_rt_rb"), new AssetID("wall_cross_lt_lb_rt"), new AssetID("wall_cross_lt_rt_lb_rb"), new AssetID("wall_doorway_horizontal_top"), new AssetID("wall_doorway_horizontal_bottom"), new AssetID("wall_doorway_vertical_top"), new AssetID("wall_doorway_vertical_bottom"), new AssetID("door_horizontal_closed_top"), new AssetID("door_horizontal_closed_bottom"), new AssetID("door_vertical_closed_top"), new AssetID("door_vertical_closed_bottom"), new AssetID("stairs_down"), new AssetID("stairs_up"), new AssetID("slime"));
        Generator.LayoutRoom[,] layout = Generator.GenerateLayout(new Random().Next(), 0, new Dictionary<RoomType, int> { { RoomType.Entrance, 1 }, { RoomType.Exit, 1 }, { RoomType.Normal, 8 } });
        Generator.TileMask[,] tilemask = Generator.GenerateTilemask(layout, type);
        (int startX, int startY) = Generator.GetStartingPosition(layout, tilemask, type);
        (int exitX, int exitY) = Generator.GetExitPosition(layout, tilemask, type);
        AssetID[,,] tilemap = Generator.GenerateTilemap(tilemask, type);
        Generator.WallBox[] walls = Generator.GenerateWalls(tilemask, type);
        int[,,] data = new int[tilemap.GetLength(0), tilemap.GetLength(1), tilemap.GetLength(2)];
        for (int z = 0; z < tilemap.GetLength(0); z++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
                for (int x = 0; x < tilemap.GetLength(2); x++)
                    data[z, y, x] = tilemap[z, y, x].Value;
        (float, float, float, float)[] wallsData = new (float, float, float, float)[walls.Length];
        for (int i = 0; i < walls.Length; i++)
            wallsData[i] = (walls[i].X, walls[i].Y, walls[i].Width, walls[i].Height);
        _ = connection.SendAsync(new LoadWorldResponsePacket() { Tilemap = data, Midground = type.MidgroundLayer, Walls = wallsData, PlayerPosition = (startX, startY), Exit = (exitX, exitY) });

        (float X, float Y, AssetID Type)[] enemies = Generator.GetEnemies(layout, tilemask, type);
        foreach (var (x, y, enemyType) in enemies)
            _ = connection.SendAsync(new SpawnEntityPacket() { EntityType = enemyType, Position = (x, y) });
    }
}