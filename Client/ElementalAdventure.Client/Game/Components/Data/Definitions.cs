using ElementalAdventure.Common.Assets;

namespace ElementalAdventure.Client.Game.Components.Data;

public readonly record struct TileType(AssetID TextureAtlas, AssetID Texture, int DepthLayerOffset, float DepthHeightOffset);

public readonly record struct EnemyType(AssetID TextureAtlas, AssetID TextureIdleLeft, AssetID TextureIdleRight, AssetID TextureWalkLeft, AssetID TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float MaxHealth, float Speed, float Damage);

public readonly record struct PlayerType(AssetID TextureAtlas, AssetID TextureIdleLeft, AssetID TextureIdleRight, AssetID TextureWalkLeft, AssetID TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float MaxHealth, float Speed);

public readonly record struct ProjectileType(AssetID TextureAtlas, AssetID Texture, int DepthLayerOffset, float DepthHeightOffset, float Damage, float Speed, bool TargetsEnemies, bool TargetsPlayers);