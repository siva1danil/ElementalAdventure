using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Server.World;

public class IsometricWorldType : IWorldType {
    private const int LayerFloor = 0, LayerDoors = 1, LayerWall = 2;

    public int RoomWidth { get; private init; }
    public int RoomHeight { get; private init; }
    public int LayerCount => 3;

    public AssetID Floor { get; private init; }

    public IsometricWorldType(int roomWidth, int roomHeight, AssetID floor) {
        RoomWidth = roomWidth;
        RoomHeight = roomHeight;
        Floor = floor;
    }

    public void MapMaskToLayers(AssetID[,,] layer, Generator.TileMask[,] mask) {
        if (layer.GetLength(0) != LayerCount)
            throw new ArgumentException($"Layer count mismatch: expected {LayerCount}, got {layer.GetLength(0)}");
        if (layer.GetLength(1) != mask.GetLength(0) || layer.GetLength(2) != mask.GetLength(1))
            throw new ArgumentException($"Mask size mismatch: expected {mask.GetLength(0)}x{mask.GetLength(1)}, got {layer.GetLength(1)}x{layer.GetLength(2)}");

        for (int y = 1; y < mask.GetLength(0) - 1; y++) {
            for (int x = 1; x < mask.GetLength(1) - 1; x++) {
                // TileMask.Floor => Floor
                if (mask[y, x] == Generator.TileMask.Floor)
                    layer[LayerFloor, y, x] = Floor;
            }
        }
    }
}