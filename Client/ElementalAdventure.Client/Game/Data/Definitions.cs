using ElementalAdventure.Client.Core.Assets;

namespace ElementalAdventure.Client.Game.Data;

public record class TileType(AssetID TextureAtlas, AssetID Texture, int DepthLayerOffset, float DepthHeightOffset);

public record class EnemyType(AssetID TextureAtlas, AssetID TextureIdleLeft, AssetID TextureIdleRight, AssetID TextureWalkLeft, AssetID TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float Speed);

public record class PlayerType(AssetID TextureAtlas, AssetID TextureIdleLeft, AssetID TextureIdleRight, AssetID TextureWalkLeft, AssetID TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float Speed);