namespace ElementalAdventure.Client.Game.Data;

public record class TileType(string TextureAtlas, string Texture);

public record class EnemyType(string TextureAtlas, string TextureIdleLeft, string TextureIdleRight, string TextureWalkLeft, string TextureWalkRight, float Speed);

public record class PlayerType(string TextureAtlas, string TextureIdleLeft, string TextureIdleRight, string TextureWalkLeft, string TextureWalkRight, float Speed);