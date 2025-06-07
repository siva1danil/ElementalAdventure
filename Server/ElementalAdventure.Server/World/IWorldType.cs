using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Server.World;

public interface IWorldType {
    public int RoomWidth { get; }
    public int RoomHeight { get; }
    public int LayerCount { get; }
    public int MidgroundLayer { get; }
    public void MapMaskToLayers(AssetID[,,] layer, Generator.TileMask[,] mask);
    public AssetID GetEnemy();
}