namespace ElementalAdventure.Server.World;

public class WorldType {
    public int RoomWidth { get; }
    public int RoomHeight { get; }
    public AssetID[] FloorTiles { get; }
    public AssetID[] WallTilesTop { get; }
    public AssetID[] WallTilesRight { get; }
    public AssetID[] WallTilesBottom { get; }
    public AssetID[] WallTilesLeft { get; }

    public WorldType(int roomWidth, int roomHeight, AssetID[] floorTiles, AssetID[] wallTilesTop, AssetID[] wallTilesRight, AssetID[] wallTilesBottom, AssetID[] wallTilesLeft) {
        RoomWidth = roomWidth;
        RoomHeight = roomHeight;
        FloorTiles = floorTiles;
        WallTilesTop = wallTilesTop;
        WallTilesRight = wallTilesRight;
        WallTilesBottom = wallTilesBottom;
        WallTilesLeft = wallTilesLeft;
    }
}