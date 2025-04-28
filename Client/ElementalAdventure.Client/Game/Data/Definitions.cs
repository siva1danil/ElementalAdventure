namespace ElementalAdventure.Client.Game.Data;

public record class ShaderLayout(Type GlobalType, Type InstanceType, Type UniformType);

public record class TileType(string TextureAtlas, string Texture, int DepthLayerOffset, float DepthHeightOffset);

public record class EnemyType(string TextureAtlas, string TextureIdleLeft, string TextureIdleRight, string TextureWalkLeft, string TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float Speed);

public record class PlayerType(string TextureAtlas, string TextureIdleLeft, string TextureIdleRight, string TextureWalkLeft, string TextureWalkRight, int DepthLayerOffset, float DepthHeightOffset, float Speed);