using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Server.World;

public interface IWorldType {
    int RoomWidth { get; }
    int RoomHeight { get; }
    int LayerCount { get; }
    public void MapMaskToLayers(AssetID[,,] layer, Generator.TileMask[,] mask);
}